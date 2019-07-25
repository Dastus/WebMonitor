using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks;
using Monitor.Application.MonitoringChecks.CommandHandlers;
using Monitor.Application.MonitoringChecks.Decorators;
using Monitor.Infrastructure.Http;
using Monitor.Infrastructure.Scheduler;
using Monitor.Infrastructure.Selenium;
using Monitor.Infrastructure.SignalR;
using Monitor.Persistence.Repository;
using System.Reflection;

namespace Monitor.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IHttpRequestService, HttpRequestService>();
            services.AddTransient<ICommandsProcessor, CommandsProcessor>();
            services.AddSingleton<IChecksRepository, MemoryCheckRepository>();
            services.AddSingleton<IScheduleRepository, MemoryScheduleRepository>();
            services.AddTransient<ISignalRNotificationsService, SignalRNotificationsService>();
            services.AddSingleton<IHostedService, SchedulerService>();
            services.AddTransient<IWebDriversFactory, SeleniumDriversFactory>();

            services.AddMediatR(typeof(HomePageCheckHandler).GetTypeInfo().Assembly);            
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandExceptionsDecorator<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggerDecorator<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SignalRDecorator<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DataStoreDecorator<,>));

            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
