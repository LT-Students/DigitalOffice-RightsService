using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.RightsService.Broker.Consumers;
using LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.RightsService.Models.Dto.Configurations;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LT.DigitalOffice.RightsService
{
  public class Startup : BaseApiInfo
  {
    public const string CorsPolicyName = "LtDoCorsPolicy";

    private readonly BaseServiceInfoConfig _serviceInfoConfig;
    private readonly RabbitMqConfig _rabbitMqConfig;

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      _serviceInfoConfig = Configuration
        .GetSection(BaseServiceInfoConfig.SectionName)
        .Get<BaseServiceInfoConfig>();

      _rabbitMqConfig = Configuration
        .GetSection(BaseRabbitMqConfig.SectionName)
        .Get<RabbitMqConfig>();

      Version = "1.3.3.0";
      Description = "RightsService is an API intended to work with the user rights.";
      StartTime = DateTime.UtcNow;
      ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddPolicy(
          CorsPolicyName,
            builder =>
            {
              builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
      });

      services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
      services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
      services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));

      services.AddHttpContextAccessor();
      services.AddMemoryCache();
      services.AddControllers()
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddNewtonsoftJson();

      string connStr = Environment.GetEnvironmentVariable("ConnectionString");

      if (string.IsNullOrEmpty(connStr))
      {
        connStr = Configuration.GetConnectionString("SQLConnectionString");
      }

      services.AddDbContext<RightsServiceDbContext>(options =>
      {
        options.UseSqlServer(connStr);
      });

      services.AddBusinessObjects();

      ConfigureMassTransit(services);

      services
        .AddHealthChecks()
        .AddSqlServer(connStr)
        .AddRabbitMqCheck();
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
      UpdateDatabase(app);

      app.UseForwardedHeaders();

      app.UseExceptionsHandler(loggerFactory);

      app.UseApiInformation();

      app.UseRouting();

      app.UseMiddleware<TokenMiddleware>();

      app.UseCors(CorsPolicyName);

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers().RequireCors(CorsPolicyName);

        endpoints.MapHealthChecks($"/{_serviceInfoConfig.Id}/hc", new HealthCheckOptions
        {
          ResultStatusCodes = new Dictionary<HealthStatus, int>
          {
            { HealthStatus.Unhealthy, 200 },
            { HealthStatus.Healthy, 200 },
            { HealthStatus.Degraded, 200 },
          },
          Predicate = check => check.Name != "masstransit-bus",
          ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
      });
    }

    private void UpdateDatabase(IApplicationBuilder app)
    {
      using var scope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();

      using var context = scope.ServiceProvider.GetService<RightsServiceDbContext>();

      context.Database.Migrate();
    }

    #region configure masstransit

    private (string username, string password) GetRabbitMqCredentials()
    {
      static string GetString(string envVar, string formAppsettings, string generated, string fieldName)
      {
        string str = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrEmpty(str))
        {
          str = formAppsettings ?? generated;

          Log.Information(
            formAppsettings == null
              ? $"Default RabbitMq {fieldName} was used."
              : $"RabbitMq {fieldName} from appsetings.json was used.");
        }
        else
        {
          Log.Information($"RabbitMq {fieldName} from environment was used.");
        }

        return str;
      }

      return (GetString("RabbitMqUsername", _rabbitMqConfig.Username, $"{_serviceInfoConfig.Name}_{_serviceInfoConfig.Id}", "Username"),
        GetString("RabbitMqPassword", _rabbitMqConfig.Password, _serviceInfoConfig.Id, "Password"));
    }

    private void ConfigureMassTransit(IServiceCollection services)
    {


      services.AddMassTransit(busConfigurator =>
      {
        busConfigurator.AddConsumer<AccessValidatorConsumer>();
        busConfigurator.AddConsumer<ChangeUserRoleConsumer>();
        busConfigurator.AddConsumer<GetUserRolesConsumer>();
        busConfigurator.AddConsumer<DisactivateUserConsumer>();

        busConfigurator.UsingRabbitMq((context, cfg) =>
        {
          cfg.Host(_rabbitMqConfig.Host, "/", host =>
          {
            host.Username($"{_serviceInfoConfig.Name}_{_serviceInfoConfig.Id}");
            host.Password(_serviceInfoConfig.Id);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.CheckUserRightsEndpoint, ep =>
          {
            ep.ConfigureConsumer<AccessValidatorConsumer>(context);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.ChangeUserRoleEndpoint, ep =>
          {
            ep.ConfigureConsumer<ChangeUserRoleConsumer>(context);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.GetUserRolesEndpoint, ep =>
          {
            ep.ConfigureConsumer<GetUserRolesConsumer>(context);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.DisactivateUserEndpoint, ep =>
          {
            ep.ConfigureConsumer<DisactivateUserConsumer>(context);
          });
        });

        busConfigurator.AddRequestClients(_rabbitMqConfig);
      });

      services.AddMassTransitHostedService();
    }
    #endregion
  }
}
