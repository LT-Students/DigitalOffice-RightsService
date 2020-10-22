using FluentValidation;
using LT.DigitalOffice.CheckRightsService.Broker.Consumers;
using LT.DigitalOffice.CheckRightsService.Business;
using LT.DigitalOffice.CheckRightsService.Business.Interfaces;
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
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.Configure<RabbitMQOptions>(Configuration);

            services.AddHealthChecks();

            services.AddControllers();

            services.AddDbContext<CheckRightsServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SQLConnectionString"));
            });

            ConfigureCommands(services);
            ConfigureMappers(services);
            ConfigureRepositories(services);
            ConfigureMassTransit(services);
            ConfigureValidators(services);

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
            string serviceName = Configuration.GetSection(RabbitMQOptions.RabbitMQ)["Username"];
            string servicePassword = Configuration.GetSection(RabbitMQOptions.RabbitMQ)["Password"];
            var rabbitMQOptions = new RabbitMQOptions();
            Configuration.GetSection(RabbitMQOptions.RabbitMQ).Bind(rabbitMQOptions);


            services.AddMassTransit(o =>
            {
                o.AddConsumer<AccessValidatorConsumer>();

                o.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", host =>
                    {
                        host.Username($"{serviceName}_{servicePassword}");
                        host.Password(servicePassword);
                    });
                });

                o.ConfigureKernelMassTransit(rabbitMQOptions);
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

        private void ConfigureValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<RightsForUserRequest>, RightsForUserValidator>();
        }
    }
}