using CarAuto.CommonRegistration;
using CarAuto.Logger;
using CarAuto.UserService.DAL.Context;
using CarAuto.UserService.Worker;
using CarAuto.UserService.Worker.BusinessLogic;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog();

var config = builder.Configuration;

foreach (var jsonFilename in Directory.EnumerateFiles("Config", "*.json", SearchOption.AllDirectories))
    config.AddJsonFile(jsonFilename);

config.AddEnvironmentVariables();

var services = builder.Services;

services.AddCommonServicesWorker<UserDbContext>(config);

// TODO: Replace with customer logic
//services.AddTransient<IUserLogic, UserLogic>();
services.AddTransient<ISyncLogic, SyncLogic>();
services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
services.AddHostedService<Worker>();

var app = builder.Build();

app.ConfigureCommonMiddlewaresWorker(app.Configuration);

app.MigrateDatabase();

app.Run();
