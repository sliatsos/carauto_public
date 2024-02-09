using CarAuto.Logger;
using Google.Protobuf.WellKnownTypes;
using KongRegister.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarAuto.CommonRegistration;

public delegate void MapGrpcServiceEndpoints(IEndpointRouteBuilder endpointRouteBuilder);
public delegate void AfterRouting(IApplicationBuilder applicationBuilder);

public static class CommonMiddleware
{
    public static event MapGrpcServiceEndpoints MapGrpcServicesEndpointsEvent;

    public static IApplicationBuilder RegisterMiddlewares(this IApplicationBuilder app)
    {
        app.AddSerilogMiddlewares();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
        app.UseCors();

        // app.UseMiddleware<CorrelationMiddlewareWrapper>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            MapGrpcServicesEndpointsEvent?.Invoke(endpoints);
            endpoints.MapGrpcReflectionService();
        });

        // Add Inadea.Logging middlewares
        // app.AddSerilogMiddlewares();

        // Enable GRPC metrics
        // app.ConfigureMetricsMiddleware(configuration);

        // Add Inadea.ApiGateway.KongRegister background service
        app.UseKongRegisterController();

        return app;
    }

    /// <summary>
    /// This methods adds commonly used middlewares to ApplicationBuilder for a Worker type microservice.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder ConfigureCommonMiddlewaresWorker(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.AddSerilogMiddlewares();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
        });

        // Add Inadea.Logging middlewares
        // app.AddSerilogMiddlewares();

        // Enable GRPC metrics
        // app.ConfigureMetricsMiddleware(configuration);

        return app;
    }

    public static IHost MigrateDatabase(this IHost app)
    {
        // Apply migrations
        using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
        {
            DbContext? context = null;
            try
            {
                context = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
            }
            catch
            {
                return app;
            }

            context.Database.Migrate();
        }

        return app;
    }
}
