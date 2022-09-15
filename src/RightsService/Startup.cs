using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DigitalOffice.Kernel.RedisSupport.Extensions;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Extensions;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.BrokerSupport.Middlewares.Token;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.EFSupport.Extensions;
using LT.DigitalOffice.Kernel.EFSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
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

namespace LT.DigitalOffice.RightsService
{
  public class Startup : BaseApiInfo
  {
    public const string CorsPolicyName = "LtDoCorsPolicy";

    private readonly BaseServiceInfoConfig _serviceInfoConfig;
    private readonly RabbitMqConfig _rabbitMqConfig;

    private string _redisConnStr;

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

      Version = "1.3.9.0";
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

      string dbConnectionString = ConnectionStringHandler.Get(Configuration);

      services.AddDbContext<RightsServiceDbContext>(options =>
      {
        options.UseSqlServer(dbConnectionString);
      });

      if (int.TryParse(Environment.GetEnvironmentVariable("MemoryCacheLiveInMinutes"), out int memoryCacheLifetime))
      {
        services.Configure<MemoryCacheConfig>(options =>
        {
          options.CacheLiveInMinutes = memoryCacheLifetime;
        });
      }
      else
      {
        services.Configure<MemoryCacheConfig>(Configuration.GetSection(MemoryCacheConfig.SectionName));
      }

      if (int.TryParse(Environment.GetEnvironmentVariable("RedisCacheLiveInMinutes"), out int redisCacheLifeTime))
      {
        services.Configure<RedisConfig>(options =>
        {
          options.CacheLiveInMinutes = redisCacheLifeTime;
        });
      }
      else
      {
        services.Configure<RedisConfig>(Configuration.GetSection(RedisConfig.SectionName));
      }

      services.AddBusinessObjects();
      services.AddTransient<IRedisHelper, RedisHelper>();
      services.AddTransient<ICacheNotebook, CacheNotebook>();

      _redisConnStr = services.AddRedisSingleton(Configuration);

      ConfigureMassTransit(services);

      services
        .AddHealthChecks()
        .AddSqlServer(dbConnectionString)
        .AddRabbitMqCheck();
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
      app.UpdateDatabase<RightsServiceDbContext>();

      FlushRedisDbHelper.FlushDatabase(_redisConnStr, Cache.Rights);

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

    #region configure masstransit

    private void ConfigureMassTransit(IServiceCollection services)
    {
      (string username, string password) = RabbitMqCredentialsHelper
        .Get(_rabbitMqConfig, _serviceInfoConfig);

      services.AddMassTransit(busConfigurator =>
      {
        busConfigurator.AddConsumer<AccessValidatorConsumer>();
        busConfigurator.AddConsumer<CreateUserRoleConsumer>();
        busConfigurator.AddConsumer<GetUserRolesConsumer>();
        busConfigurator.AddConsumer<DisactivateUserRoleConsumer>();
        busConfigurator.AddConsumer<ActivateUserRoleConsumer>();
        busConfigurator.AddConsumer<FilterRolesUsersConsumer>();

        busConfigurator.UsingRabbitMq((context, cfg) =>
        {
          cfg.Host(_rabbitMqConfig.Host, "/", host =>
          {
            host.Username(username);
            host.Password(password);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.CheckUserRightsEndpoint, ep =>
          {
            ep.ConfigureConsumer<AccessValidatorConsumer>(context);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.CreateUserRoleEndpoint, ep =>
          {
            ep.ConfigureConsumer<CreateUserRoleConsumer>(context);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.GetUserRolesEndpoint, ep =>
          {
            ep.ConfigureConsumer<GetUserRolesConsumer>(context);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.DisactivateUserRoleEndpoint, ep =>
          {
            ep.ConfigureConsumer<DisactivateUserRoleConsumer>(context);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.ActivateUserRoleEndpoint, ep =>
          {
            ep.ConfigureConsumer<ActivateUserRoleConsumer>(context);
          });

          cfg.ReceiveEndpoint(_rabbitMqConfig.FilterRolesEndpoint, ep =>
          {
            ep.ConfigureConsumer<FilterRolesUsersConsumer>(context);
          });
        });

        busConfigurator.AddRequestClients(_rabbitMqConfig);
      });

      services.AddMassTransitHostedService();
    }
    #endregion
  }
}
