using FluentValidation;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.CheckRightsService.Broker.Consumers;
using LT.DigitalOffice.CheckRightsService.Business;
using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
using LT.DigitalOffice.CheckRightsService.Configuration;
using LT.DigitalOffice.CheckRightsService.Data;
using LT.DigitalOffice.CheckRightsService.Data.Interfaces;
using LT.DigitalOffice.CheckRightsService.Data.Provider;
using LT.DigitalOffice.CheckRightsService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.CheckRightsService.Mappers;
using LT.DigitalOffice.CheckRightsService.Mappers.Interfaces;
using LT.DigitalOffice.CheckRightsService.Models.Db;
using LT.DigitalOffice.CheckRightsService.Models.Dto;
using LT.DigitalOffice.CheckRightsService.Validation;
using LT.DigitalOffice.Kernel;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.CheckRightsService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));

            services.AddHealthChecks();

            services.AddControllers();

            string connStr = Environment.GetEnvironmentVariable("ConnectionString");
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = Configuration.GetConnectionString("SQLConnectionString");
            }

            services.AddDbContext<CheckRightsServiceDbContext>(options =>
            {
                options.UseSqlServer(connStr);
            });

            ConfigureCommands(services);
            ConfigureValidator(services);
            ConfigureMappers(services);
            ConfigureRepositories(services);
            ConfigureMassTransit(services);

            services.AddKernelExtensions();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthChecks("/api/healthcheck");

            app.UseExceptionHandler(tempApp => tempApp.Run(CustomExceptionHandler.HandleCustomException));

            UpdateDatabase(app);

#if RELEASE
            app.UseHttpsRedirection();
#endif

            app.UseRouting();

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
            });
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var context = scope.ServiceProvider.GetService<CheckRightsServiceDbContext>();

            context.Database.Migrate();
        }

        private void ConfigureMassTransit(IServiceCollection services)
        {
            var rabbitMqConfig = Configuration
                .GetSection(BaseRabbitMqOptions.RabbitMqSectionName)
                .Get<RabbitMqConfig>();

            services.AddMassTransit(o =>
            {
                o.AddConsumer<AccessValidatorConsumer>();

                o.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConfig.Host, "/", host =>
                    {
                        host.Username($"{rabbitMqConfig.Username}_{rabbitMqConfig.Password}");
                        host.Password(rabbitMqConfig.Password);
                    });

                    cfg.ReceiveEndpoint(rabbitMqConfig.CheckUserRightsEndpoint, ep =>
                    {
                        ep.ConfigureConsumer<AccessValidatorConsumer>(context);
                    });
                });

                o.AddRequestClient<ICheckTokenRequest>(
                    new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.ValidateTokenEndpoint}"));

                o.AddRequestClient<IGetUserRequest>(
                    new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetUserInfoEndpoint}"));

                o.ConfigureKernelMassTransit(rabbitMqConfig);
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
            services.AddTransient<IDataProvider, CheckRightsServiceDbContext>();
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
    }
}