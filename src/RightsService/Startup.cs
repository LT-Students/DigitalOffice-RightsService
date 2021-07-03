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
using System;
using System.Collections.Generic;

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

            Version = "1.3.0";
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
                            .WithOrigins(
                                "https://*.ltdo.xyz",
                                "http://*.ltdo.xyz",
                                "http://ltdo.xyz",
                                "http://ltdo.xyz:9802",
                                "http://localhost:4200",
                                "http://localhost:4500")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
            services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
            services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddControllers();

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

        private void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumer<AccessValidatorConsumer>();
                busConfigurator.AddConsumer<AccessCollectionValidatorConsumer>();

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
                });

                busConfigurator.AddRequestClients(_rabbitMqConfig);
            });

            services.AddMassTransitHostedService();
        }
    }
}
