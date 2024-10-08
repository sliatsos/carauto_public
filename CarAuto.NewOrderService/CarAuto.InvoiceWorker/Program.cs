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
using CarAuto.OrderService.Mappings;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog();

var config = builder.Configuration;

foreach (var jsonFilename in Directory.EnumerateFiles("Config", "*.json", SearchOption.AllDirectories))
    config.AddJsonFile(jsonFilename);

config.AddEnvironmentVariables();

var services = builder.Services;

var numberConfig = new NumberConfig();
config.GetSection("Number").Bind(numberConfig);

services.RegisterServices<OrderDbContext>(config);
services.AddAutoMapper(typeof(MappingProfile));
services.AddTransient<IOrderLogic, OrderLogic>();
services.AddTransient<ISequenceLogic, SequenceLogic>();
services.AddTransient<IProducerFactory, ProducerFactory>();
services.AddSingleton(numberConfig);
services.AddHostedService<InvoiceWorker>();
services.AddSignalR();
services.Configure<HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = null;
});
var app = builder.Build();

app.MapHub<InvoiceToOrderHub>("/invoiceToOrder");

app.RegisterMiddlewares();

app.Run();
