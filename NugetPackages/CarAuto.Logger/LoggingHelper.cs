using CarAuto.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Kafka;
using System;

namespace CarAuto.Logger;
public static class LoggingHelper
{
    public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
             => hostBuilder
            .UseSerilog();

    public static IServiceCollection AddSerilogServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        KafkaConfiguration kafkaConfiguration = configuration.GetSection("Serilog:Kafka").Get<KafkaConfiguration>();

        if (kafkaConfiguration != null)
        {
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Kafka(
                    bootstrapServers: kafkaConfiguration.BootstrapServers,
                    topic: kafkaConfiguration.Topic,
                    batchSizeLimit: kafkaConfiguration.BatchSizeLimit,
                    period: kafkaConfiguration.Period,
                    formatter: new JsonFormatterWithMaxLength());
            if (configuration.GetValue<bool>("Serilog:LogToConsole"))
            {
                loggerConfig = loggerConfig.WriteTo.Console(new JsonFormatterWithMaxLength());
            }

            Log.Logger = loggerConfig.CreateLogger();

            Log.Logger.Information("Serilog Kafka Sink configured successfully.");
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
                                       .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
                                       .WriteTo.Console()
                                       .CreateLogger();

            Log.Logger.Error("Serilog is not configured. Configurations are missing! File logging Enabled!");
        }

        services.AddSingleton(Log.Logger);
        AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
        return services;
    }

    public static IApplicationBuilder AddSerilogMiddlewares(this IApplicationBuilder applicationBuilder)
       => applicationBuilder.UseSerilogRequestLogging();
}