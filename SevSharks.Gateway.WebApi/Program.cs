using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SolarLab.BusManager.Abstraction;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SevSharks.Gateway.WebApi
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">args</param>
        public static void Main(string[] args)
        {
            var host = CreateWebHost(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            var services = host.Services;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Gateway started");
            try
            {
                var busManager = services.GetService<IBusManager>();
                busManager.StartBus(new Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during start bus for gateway");
            }

            host.Run();
        }

        /// <summary>
        /// CreateWebHost
        /// </summary>
        public static IWebHost CreateWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseKestrel()
                   .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        //config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        config.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
                        config.AddJsonFile("secrets/appsettings.secrets.json", optional: true);
                        config.AddEnvironmentVariables();
                        config.AddCommandLine(args);
                    })
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddSerilog();
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                    })
                   .UseStartup<Startup>()
                   .UseSerilog()
                   .Build();
    }
}