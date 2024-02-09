using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using CommunityToolkit.Maui;
using CarAuto.MAUI.View;
using CarAuto.MAUI.Shared.Services;
using CarAuto.MAUI.Shared.ViewModels;

namespace CarAuto.MAUI;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkit();

        var services = builder.Services;
        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<IToastManager, ToastManager>();
        services.AddSingleton<MainPage>();
        services.AddSingleton<Shell>();
        services.AddSingleton<UserViewModel>();
        services.AddSingleton<App>();
        services.AddSingleton<UserRegisterView>();
        services.AddSingleton(services =>
        {
            SocketsHttpHandler handler = new SocketsHttpHandler();
            handler.Proxy = new WebProxy();
            HttpClient httpClient = new HttpClient(handler);
            return GrpcChannel.ForAddress("http://7.tcp.eu.ngrok.io:17834/CarAuto.User.UserService/CreateUser", new GrpcChannelOptions()
            {HttpClient = httpClient});
        //return GrpcChannel.ForAddress("http://2.tcp.eu.ngrok.io:14952/CarAuto.User.UserService/CreateUser", new GrpcChannelOptions
        //{
        //	HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        //	{
        //		HttpVersion = new Version(2,0),
        //	},
        //});
        });
        return builder.Build();
    }
}