// See https://aka.ms/new-console-template for more information
using CarAuto.CommonRegistration;
using CarAuto.Kafka;
using CarAuto.Logger;
using CarAuto.NewOrderService.Hubs;
using CarAuto.NewOrderService.Workers;
using CarAuto.OrderService.Business;
using CarAuto.OrderService.Business.Interfaces;
using CarAuto.OrderService.DAL.Context;
using CarAuto.OrderService.DAL.DTOs;
using CarAuto.OrderService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog();

var config = builder.Configuration;

foreach (var jsonFilename in Directory.EnumerateFiles("Config", "*.json", SearchOption.AllDirectories))
    config.AddJsonFile(jsonFilename);

var services = builder.Services;

var numberConfig = new NumberConfig();
config.GetSection("Number").Bind(numberConfig);

services.RegisterServices<OrderDbContext>(config);
services.AddTransient<IOrderLogic, OrderLogic>();
services.AddTransient<ISequenceLogic, SequenceLogic>();
services.AddTransient<IProducerFactory, ProducerFactory>();
services.AddSingleton(numberConfig);
services.AddHostedService<OrderWorker>();
services.AddSignalR();

var app = builder.Build();

CommonMiddleware.MapGrpcServicesEndpointsEvent += (endpoints) =>
{
    endpoints.MapGrpcService<OrderService>().RequireCors("AllowAll");
};

app.MapHub<QuoteToOrderHub>("/quoteToOrder");

app.RegisterMiddlewares();

app.MigrateDatabase();

app.Run();
