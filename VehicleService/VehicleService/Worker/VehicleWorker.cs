using CarAuto.Kafka.Json;
using CarAuto.Kafka;
using CarAuto.Protos.Vehicle;
using Confluent.Kafka;
using CarAuto.VehicleService.Business.Interfaces;
using CarAuto.VehicleService.DAL.DTOs;

namespace CarAuto.VehicleService.Worker
{
    public class VehicleWorker : BackgroundConsumer<string, VehicleDto, JsonDeserializer<VehicleDto>>
    {
        public VehicleWorker(
            ILogger<BackgroundConsumer<string, VehicleDto, JsonDeserializer<VehicleDto>>> logger,
            IConfiguration configuration,
            IHostApplicationLifetime hostApplicationLifetime,
            IServiceScopeFactory serviceScopeFactory) : 
            base(logger, configuration, hostApplicationLifetime, serviceScopeFactory)
        {
        }

        public override string Topics => "dev.general.vehicles.json";

        protected override async Task ProcessMessage(ConsumeResult<string, VehicleDto> consumeResult, IServiceScope scope)
        {
            var vehicleLogic = scope.ServiceProvider.GetService<IVehicleLogic>();
            await vehicleLogic!.CreateVehicleAsync(consumeResult.Message.Value);
        }
    }
}
