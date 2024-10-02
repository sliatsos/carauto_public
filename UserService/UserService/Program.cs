using CarAuto.CommonRegistration;
using CarAuto.Kafka;
using CarAuto.Logger;
using CarAuto.UserService.Business;
using CarAuto.UserService.Business.Interfaces;
using CarAuto.UserService.DAL.Context;
using CarAuto.UserService.Services;
using CarAuto.UserService.Utils;
using Keycloak.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog();

var config = builder.Configuration;

foreach (var jsonFilename in Directory.EnumerateFiles("Config", "*.json", SearchOption.AllDirectories))
    config.AddJsonFile(jsonFilename);

config.AddEnvironmentVariables();

var services = builder.Services;

services.RegisterServices<UserDbContext>(config);

services.AddTransient<IUserLogic, UserLogic>();
services.AddTransient<ICustomerLogic, CustomerLogic>();
services.AddTransient<IProducerFactory, ProducerFactory>();
services.AddTransient<ISalespersonLogic, SalespersonLogic>();

var keycloakConfig = config.GetSection("Keycloak").Get<KeycloakConfig>();
var keyCloakServiceUri = new Uri(keycloakConfig.DiscoveryServiceUrl);
var keyCloakClient = new KeycloakClient($"{keyCloakServiceUri.Scheme}://{keyCloakServiceUri.Host}:{keyCloakServiceUri.Port}", keycloakConfig.KeyCloakAdminUser, keycloakConfig.KeyCloakAdminUserPassword);
services.AddSingleton(keyCloakClient);
services.AddSingleton(keycloakConfig);

var app = builder.Build();

CommonMiddleware.MapGrpcServicesEndpointsEvent += (endpoints) =>
{
    endpoints.MapGrpcService<UserService>().RequireCors("AllowAll");
    endpoints.MapGrpcService<CustomerService>().RequireCors("AllowAll");
    endpoints.MapGrpcService<SalespersonService>().RequireCors("AllowAll");
};

app.RegisterMiddlewares();

app.MigrateDatabase();

app.Run();
