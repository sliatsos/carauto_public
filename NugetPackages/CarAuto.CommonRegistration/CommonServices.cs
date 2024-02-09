using CarAuto.Caching;
using CarAuto.ClaimParser;
using CarAuto.EFCore.BaseEntity;
using CarAuto.EFCore.BaseEntity.Repositories;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Kong;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using KongRegister.Business.Interfaces;
using KongRegister.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using CarAuto.Kafka;
using CarAuto.Kafka.Config;
using Confluent.Kafka;
using CarAuto.Logger;

namespace CarAuto.CommonRegistration;

public static class CommonServices
{
    public static IServiceCollection RegisterServices<T>(this IServiceCollection? services, IConfiguration config)
        where T : CustomDbContext
    {
        RegisterServices(services, config);
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddDbContext<DbContext, T>(optionsBuilder =>
            optionsBuilder.UseNpgsql(config.GetConnectionString("Database"))
            .UseSnakeCaseNamingConvention());
        services.AddTransient<IClaimParser, ClaimParser.ClaimParser>();
        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection? services, IConfiguration config)
    {
        services.AddGrpc(o => o.Interceptors.Add<ExceptionInterceptor.ExceptionInterceptor>());

        services.AddCors(o => o.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));

        services.AddHealthChecks();
        services.AddGrpcReflection();
        services.AddRedisCache(config);
        services.AddSerilogServices(config);
        services.AddDefaultCorrelationId(options =>
        {
            options.AddToLoggingScope = true;
        });
        services.AddHttpClient(string.Empty).AddCorrelationIdForwarding();
        services.AddHeaderPropagation(e =>
        {
            e.Headers.Add("Authorization");
        });

        services.AddSingleton(typeof(KafkaConfig));

        services.AddKong(config);
        services.AddSingleton<IKongRegisterBusiness, Kong.KongRegister>();
        services.AddAutoMapper(Assembly.GetEntryAssembly());

        var authenticationOptions = config
            .GetSection(KeycloakAuthenticationOptions.Section)
            .Get<KeycloakAuthenticationOptions>();
        var adminClientOptions = config
            .GetSection(KeycloakAdminClientOptions.Section)
            .Get<KeycloakAdminClientOptions>();
        var authorizationOptions = config
            .GetSection(KeycloakProtectionClientOptions.Section)
            .Get<KeycloakProtectionClientOptions>();

        services.AddKeycloakAuthentication(authenticationOptions);

        services
            .AddAuthorization(o =>
            {
                o.AddPolicy(AuthorizationPolicies.IsAdmin, b =>
                {
                    b.RequireRealmRoles(AuthorizationRoles.Admin);
                    b.RequireResourceRoles(AuthorizationRoles.Admin);
                    b.RequireRole(AuthorizationRoles.Admin);
                });

                o.AddPolicy(AuthorizationPolicies.IsUser, b =>
                {
                    b.RequireRealmRoles(AuthorizationRoles.User);
                    b.RequireResourceRoles(AuthorizationRoles.User);
                    b.RequireRole(AuthorizationRoles.User);
                });

                o.AddPolicy(AuthorizationPolicies.IsPowerUser, b =>
                {
                    b.RequireRealmRoles(AuthorizationRoles.PowerUser);
                    b.RequireResourceRoles(AuthorizationRoles.PowerUser);
                    b.RequireRole(AuthorizationRoles.PowerUser);
                });
            })
            .AddKeycloakAuthorization(authorizationOptions);

        services.AddKeycloakAdminHttpClient(adminClientOptions);

        return services;
    }

    public static IServiceCollection AddCommonServicesWorker<T>(this IServiceCollection services, IConfiguration configuration)
        where T : CustomDbContext
    {
        AddCommonServicesWorker(services, configuration);
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddDbContext<DbContext, T>(optionsBuilder =>
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"))
                .UseSnakeCaseNamingConvention());
        services.AddTransient<IClaimParser, ClaimParser.ClaimParser>();
        return services;
    }

    /// <summary>
    /// Adds common services to a microservice of type background worker.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommonServicesWorker(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.AddSingleton(typeof(KafkaConfig));

        services.AddSerilogServices(configuration);
        // add grpc
        services.AddGrpc();

        // add health checks
        services.AddHealthChecks();

        services.AddHttpClient(string.Empty);

        // add grpc client wrapper
        //services.AddTransient<IGrpcClientWrapper, GrpcClientWrapper>();

        // Add Redis Caching
        services.AddRedisCache(configuration);

        // Add Incadea.Logging services
        //services.AddSerilogServices(configuration);

        // Register DI
        services.AddScoped<IProducerFactory, ProducerFactory>();
        services.AddDefaultCorrelationId(options =>
        {
            options.AddToLoggingScope = true;
        });
        services.AddHttpClient(string.Empty).AddCorrelationIdForwarding();
        services.AddHeaderPropagation(e =>
        {
            e.Headers.Add("Authorization");
        });

        services.AddAutoMapper(Assembly.GetEntryAssembly());

        return services;
    }
}