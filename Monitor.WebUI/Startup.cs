using MediatR;
using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.CommandHandlers;
using Monitor.Application.MonitoringChecks.Decorators;
using Monitor.Infrastructure.Http;
using Monitor.Infrastructure.Scheduler;
using Monitor.Infrastructure.Selenium;
using Monitor.Infrastructure.SignalR;
using Monitor.Infrastructure.Processors;
using Monitor.Persistence.Repository;
using System.Reflection;
using Monitor.Infrastructure.Registrator;
using Monitor.Infrastructure.Logger;
using Monitor.Infrastructure.Telegram;
using Monitor.Application.MonitoringChecks.ResultsHandlingLogic;
using Monitor.Infrastructure.Settings;
using System;
using AutoMapper;
using Monitor.Infrastructure.Mappings;
using Monitor.Infrastructure.ExternalUnitTests;
using Monitor.Infrastructure.Notifications;

namespace Monitor.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //TODO: configure Lamar "WithDefaultConventions" to remove unneeded registrations 
        public void ConfigureContainer(ServiceRegistry services)
        {
            services.AddSignalR();
            services.AddMvc();

            services.Scan(s => {
                //s.TheCallingAssembly();
                //s.WithDefaultConventions();
                s.AssemblyContainingType(typeof(HomePageCheckHandler));
                s.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                s.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            });
            
            services.AddSingleton<ICommandsProcessor, CommandsProcessor>();
            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddSingleton<IChecksRepository, MemoryCheckRepository>();
            services.AddSingleton<IScheduleRepository, MemoryScheduleRepository>();
            services.AddSingleton<IHostedService, CheckRegistrator>();
            services.AddSingleton<ILoggerService, TextLoggerService>();
            services.AddSingleton<IHttpRequestService, HttpRequestService>();
            services.AddSingleton<INotificationsService, NotificationsService>();

            services.AddTransient<ISignalRNotificationsService, SignalRNotificationsService>();
            services.AddTransient<IResultHandlingService, ResultHandlingService>();
            services.AddTransient<IUnitTestsProcessorService, UnitTestsProcessorService>();
            services.AddTransient<IWebDriversFactory, SeleniumDriversFactory>();
            services.AddTransient<ITelegramNotificationService, TelegramService>();

            services.AddMediatR(typeof(HomePageCheckHandler).GetTypeInfo().Assembly);            

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggerDecorator<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandExceptionsDecorator<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandResultHandleDecorator<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SignalRDecorator<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NotificationsDecorator<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DataStoreDecorator<,>));

            services.Configure<TelegramNotificationSettings>(Configuration.GetSection("TelegramNotificationSettings"));
            services.Configure<LoggerSettings>(Configuration.GetSection("LoggerSettings"));

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddAutoMapper(typeof(ChecksMappingProfile));
        }

        public void Configure(IApplicationBuilder app, Microsoft.Extensions.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSpaStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<MonitoringHub>("/notify");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
