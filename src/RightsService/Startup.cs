using FluentValidation;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.RightsService.Broker.Consumers;
using LT.DigitalOffice.RightsService.Business;
using LT.DigitalOffice.RightsService.Business.Interfaces;
using LT.DigitalOffice.RightsService.Configuration;
using LT.DigitalOffice.RightsService.Data;
using LT.DigitalOffice.RightsService.Data.Interfaces;
using LT.DigitalOffice.RightsService.Data.Provider;
using LT.DigitalOffice.RightsService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.RightsService.Mappers;
using LT.DigitalOffice.RightsService.Mappers.Interfaces;
using LT.DigitalOffice.RightsService.Models.Db;
using LT.DigitalOffice.RightsService.Models.Dto;
using LT.DigitalOffice.RightsService.Validation;
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
        public IConfiguration Configuration { get; }

        private RabbitMqConfig _rabbitMqConfig;
        private BaseServiceInfoConfig _serviceInfoConfig;

        #region private methods
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
            services.AddMassTransit(x =>
            {
                x.AddConsumer<AccessValidatorConsumer>();
                x.AddConsumer<AccessCollectionValidatorConsumer>();

                x.UsingRabbitMq((context, cfg) =>
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

                x.AddRequestClient<ICheckTokenRequest>(
                    new Uri($"{_rabbitMqConfig.BaseUrl}/{_rabbitMqConfig.ValidateTokenEndpoint}"));

                x.AddRequestClient<IGetUserRequest>(
                    new Uri($"{_rabbitMqConfig.BaseUrl}/{_rabbitMqConfig.GetUserInfoEndpoint}"));

                x.ConfigureKernelMassTransit(_rabbitMqConfig);
            });

            services.AddMassTransitHostedService();
        }

        private void ConfigureCommands(IServiceCollection services)
        {
            services.AddTransient<IGetRightsListCommand, GetRightsListCommand>();
            services.AddTransient<IAddRightsForUserCommand, AddRightsForUserCommand>();
            services.AddTransient<IRemoveRightsFromUserCommand, RemoveRightsFromUserCommand>();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services.AddTransient<IDataProvider, RightsServiceDbContext>();
            services.AddTransient<ICheckRightsRepository, CheckRightsRepository>();
        }

        private void ConfigureMappers(IServiceCollection services)
        {
            services.AddTransient<IMapper<DbRight, Right>, RightsMapper>();
        }

        private void ConfigureValidator(IServiceCollection services)
        {
            services.AddTransient<IValidator<IEnumerable<int>>, RightsIdsValidator>();
        }

        #endregion

        #region public methods

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _rabbitMqConfig = Configuration
             .GetSection(BaseRabbitMqConfig.SectionName)
             .Get<RabbitMqConfig>();

            _serviceInfoConfig = Configuration
                .GetSection(BaseServiceInfoConfig.SectionName)
                .Get<BaseServiceInfoConfig>();

            var description = string.Join(" ",
                "RightsService is an API intended to work with the user rights:",
                "create them, assign them to people, remove.");

            Version = "1.1.2";
            Description = description;
            StartTime = DateTime.UtcNow;
            ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
            services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));

            services.AddMemoryCache();

            services.AddKernelExtensions();

            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));

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

            services
               .AddHealthChecks()
               .AddSqlServer(connStr)
               .AddRabbitMqCheck();

            ConfigureCommands(services);
            ConfigureValidator(services);
            ConfigureMappers(services);
            ConfigureRepositories(services);
            ConfigureMassTransit(services);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionsHandler(loggerFactory);

            UpdateDatabase(app);

#if RELEASE
            app.UseHttpsRedirection();
#endif

            app.UseRouting();

            app.UseApiInformation();
            app.UseMiddleware<TokenMiddleware>();

            string corsUrl = Configuration.GetSection("Settings")["CorsUrl"];

            app.UseCors(builder =>
                builder
                    .WithOrigins(corsUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

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

        #endregion
    }
}