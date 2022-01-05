using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace HangfireTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Hangfire Job";
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureAppConfiguration((hostContext, config) =>
            {
                var env = hostContext.HostingEnvironment;
                config.AddEnvironmentVariables();

                config.AddJsonFile("Configs/appsettings.json", false, true);
                config.AddJsonFile($"Configs/appsettings.{env.EnvironmentName}.json", true, true);
                config.AddJsonFile("Configs/serilog.json", false, true);
                config.AddJsonFile($"Configs/serilog.{env.EnvironmentName}.json", true, true);

                hostContext.Configuration = config.Build();
            });

            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
                    loggerConfiguration.Enrich.FromLogContext();
                    loggerConfiguration.Enrich.WithProperty("App", hostingContext.HostingEnvironment.ApplicationName);
                    loggerConfiguration.Enrich.WithProperty("Env", hostingContext.HostingEnvironment.EnvironmentName);
                });
                webBuilder.UseStartup<Startup>();
            });

            return builder;
        }
    }
}