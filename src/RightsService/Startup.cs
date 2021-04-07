using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.RightsService.Broker.Consumers;
using LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.RightsService.Models.Dto.Configurations;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace LT.DigitalOffice.RightsService
{
    public class Startup : BaseApiInfo
    {
        private readonly BaseServiceInfoConfig _serviceInfoConfig;
        private readonly RabbitMqConfig _rabbitMqConfig;
        private readonly ILogger<Startup> _logger;

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

            Version = "1.2.0";
            Description = "RightsService is an API intended to work with the user rights .";
            StartTime = DateTime.UtcNow;
            ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("LT.DigitalOffice.RightsService.Startup", LogLevel.Trace)
                    .AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddHealthChecks();
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

            services.AddBusinessObjects(_logger);

            ConfigureMassTransit(services);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            UpdateDatabase(app);

            app.UseHealthChecks("/api/healthcheck");

            app.UseExceptionsHandler(loggerFactory);

#if RELEASE
            app.UseHttpsRedirection();
#endif

            app.UseRouting();

            app.UseMiddleware<TokenMiddleware>();
            app.UseApiInformation();

            string corsUrl = Configuration.GetSection("Settings")["CorsUrl"];

            app.UseCors(builder =>
                builder
                    .WithOrigins(corsUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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

                busConfigurator.AddRequestClients(_rabbitMqConfig, _logger);
            });

            services.AddMassTransitHostedService();
        }
    }
}