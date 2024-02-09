using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CarAuto.Caching;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var configOptions = new ConfigurationOptions
        {
            EndPoints =
            {
                {
                    $"{configuration["Redis:Host"]}", int.Parse(configuration["Redis:Port"])
                },
            },
            AbortOnConnectFail = false,
            ConnectTimeout = !string.IsNullOrWhiteSpace(configuration["Redis:ConnectTimeout"]) ? int.Parse(configuration["Redis:ConnectTimeout"]) : 1000,
        };
        if (!string.IsNullOrEmpty(configuration["Redis:Password"]))
        {
            configOptions.Password = configuration["Redis:Password"];
        }

        return services.AddDistributedRedisCache(options =>
        {
            options.ConfigurationOptions = configOptions;
        })
        .AddTransient<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(configOptions))
        .AddTransient<IRedisManager, RedisManager>();
    }
}