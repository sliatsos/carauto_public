using KongRegister.Business.Interfaces;
using KongRegister.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KongRegister.Extensions;

namespace CarAuto.Kong;

public static class KongExtensions
{
    public static IServiceCollection AddKong(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.AddSingleton(typeof(KongConfigure), configuration.GetSection("KongRegister").Get<KongConfigure>());
        services.ConfigureKongRegister(configuration);
        services.AddSingleton(typeof(IKongRegisterBusiness), typeof(KongRegisterBusiness));
        return services;
    }

    public static IApplicationBuilder AddKongMiddleware(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.UseKongRegisterController();
    }
}
