// See https://aka.ms/new-console-template for more information
using CarAuto.CommonRegistration;
using CarAuto.Kafka;
using CarAuto.Logger;
using CarAuto.VehicleService.Business;
using CarAuto.VehicleService.Business.Interfaces;
using CarAuto.VehicleService.DAL.Context;
using CarAuto.VehicleService.Services;
using CarAuto.VehicleService.SignalR;
using CarAuto.VehicleService.Worker;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog();

var config = builder.Configuration;

foreach (var jsonFilename in Directory.EnumerateFiles("Config", "*.json", SearchOption.AllDirectories))
    config.AddJsonFile(jsonFilename);

var services = builder.Services;

services.RegisterServices<VehicleDbContext>(config);
services.AddTransient<IVehicleLogic, VehicleLogic>();
services.AddTransient<IBrandLogic, BrandLogic>();
services.AddTransient<IModelLogic, ModelLogic>();
services.AddTransient<IProducerFactory, ProducerFactory>();
services.AddTransient<IOptionLogic, OptionLogic>();
services.AddHostedService<VehicleWorker>();
services.AddSignalR();

var app = builder.Build();

CommonMiddleware.MapGrpcServicesEndpointsEvent += (endpoints) =>
{
    endpoints.MapGrpcService<VehicleService>().RequireCors("AllowAll");
    endpoints.MapGrpcService<BrandService>().RequireCors("AllowAll");
    endpoints.MapGrpcService<ModelService>().RequireCors("AllowAll");
    endpoints.MapGrpcService<OptionService>().RequireCors("AllowAll");
};

app.MapHub<VehicleHub>("/vehicle");

app.RegisterMiddlewares();

app.MigrateDatabase();

app.Run();
