using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Monitor.WebUI
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    CreateWebHostBuilder(args).Build().Run();
        //}

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //    .UseStartup<Startup>();

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseLamar()
            .UseStartup<Startup>();

        //public static void Main(string[] args)
        //{
        //    var builder = new WebHostBuilder();
        //    builder
        //        // Replaces the built in DI container
        //        // with Lamar
        //        .UseLamar()

        //        // Normal ASP.Net Core bootstrapping
        //        .UseUrls("https://localhost:44320/")
        //        .UseKestrel()
        //        .UseStartup<Startup>();

        //    builder.Start();

        //}

    }
}
