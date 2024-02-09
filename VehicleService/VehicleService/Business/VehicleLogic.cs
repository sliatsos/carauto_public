using AutoMapper;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Kafka.Config;
using CarAuto.Kafka;
using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.Business.Interfaces;
using CarAuto.Kafka.Json;
using CarAuto.VehicleService.DAL.DTOs;
using VehicleEntity = CarAuto.VehicleService.DAL.Entities.Vehicle;
using VehicleProto = CarAuto.Protos.Vehicle.Vehicle;
using ModelEntity = CarAuto.VehicleService.DAL.Entities.Model;
using OptionEntity = CarAuto.VehicleService.DAL.Entities.Option;
using Action = CarAuto.VehicleService.DAL.DTOs.Action;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CarAuto.VehicleService.Business;

public class VehicleLogic : IVehicleLogic
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProducerFactory _producerFactory;
    private readonly KafkaConfig _kafkaConfig;

    public VehicleLogic(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IProducerFactory producerFactory,
        KafkaConfig kafkaConfig)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _producerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
        _kafkaConfig = kafkaConfig ?? throw new ArgumentNullException(nameof(kafkaConfig));
    }

    public async Task<CreateVehicleResponse> CreateVehicleAsync(CreateVehicleRequest request)
    {
        var vehicle = _mapper.Map<VehicleEntity>(request.Vehicle);
        if (vehicle.ModelId != Guid.Empty)
        {
            var model = await _unitOfWork.GetRepository<ModelEntity>()
                .Queryable
                .Include(e => e.Options)
                .FirstOrDefaultAsync(x => x.Id == vehicle.ModelId);
            foreach (var option in model.Options)
            {
                vehicle.Options.Add(new OptionEntity
                {
                    Code = option.Code,
                    Description = option.Description,
                    InternalCode = option.InternalCode,
                    Type = option.Type,
                    UnitCost = option.UnitCost,
                    UnitPrice = option.UnitPrice,
                });
            }
        }
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<VehicleEntity>();
        await vehicleRepo.InsertAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        await SendVehicleStatusAsync(vehicle, Action.Insert);


        return new CreateVehicleResponse
        {
            Id = vehicle.Id.ToString(),
        };
    }

    public async Task CreateVehicleAsync(VehicleDto vehicle)
    {
        var optionRepo = await _unitOfWork.GetRepositoryAsync<OptionEntity>();

        var options = vehicle.Options.ToList();
        vehicle.Options.Clear();

        var vehicleEntity = _mapper.Map<VehicleEntity>(vehicle);
        vehicleEntity.RegistrationDate = vehicleEntity.RegistrationDate.ToUniversalTime();
        vehicleEntity.ModifiedOn = vehicleEntity.ModifiedOn.ToUniversalTime();
        foreach (var option in options)
        {
            var modelOption = await optionRepo.GetByIdAsync(option.Id);
            var vehOption = new OptionEntity
            {
                InternalCode = modelOption.InternalCode,
                Description = modelOption.Description,
                Code = modelOption.Code,
                Type = modelOption.Type,
                UnitCost = modelOption.UnitCost,
                UnitPrice = modelOption.UnitPrice,
            };
            vehicleEntity.Options.Add(vehOption);
        }
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<VehicleEntity>();
        await vehicleRepo.InsertAsync(vehicleEntity);
        await _unitOfWork.SaveChangesAsync();

        await SendVehicleStatusAsync(vehicleEntity, Action.Insert);
    }

    public async Task DeleteVehicleAsync(DeleteVehicleRequest request)
    {
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<VehicleEntity>();
        var vehicle = await vehicleRepo.GetByIdAsync(Guid.Parse(request.Id));
        await SendVehicleStatusAsync(vehicle, Action.Delete);
        vehicleRepo.Delete(vehicle);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GetAllVehiclesResponse> GetAllVehiclesAsync()
    {
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<VehicleEntity>();

        var result = new GetAllVehiclesResponse();
        result.Vehicles.AddRange(vehicleRepo.Queryable.Select(e => _mapper.Map<VehicleProto>(e)));
        return result;
    }

    public async Task<GetVehicleResponse> GetVehicleAsync(GetVehicleRequest request)
    {
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<VehicleEntity>();
        var vehicle = vehicleRepo.Queryable
            .Include(e => e.Options)
            .FirstOrDefault(e => e.Id == Guid.Parse(request.Id));

        return new GetVehicleResponse
        {
            Vehicle = _mapper.Map<VehicleProto>(vehicle),
        };
    }

    public Task<SearchVehiclesResponse> SearchVehiclesAsync(SearchVehiclesRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateVehicleAsync(UpdateVehicleRequest request)
    {
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<VehicleEntity>();
        var vehicle = await vehicleRepo.GetByIdAsync(Guid.Parse(request.Vehicle.Id));
        vehicle = _mapper.Map(request.Vehicle, vehicle);
        await SendVehicleStatusAsync(vehicle, Action.Update);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SendVehicleStatusAsync(VehicleEntity vehicle, Action action)
    {
        var dataSync = new DataSynchronizationDto
        {
            Action = action,
            Entity = nameof(PayloadDto.Vehicles),
            Payload = new PayloadDto
            {
                Vehicles = new List<VehicleDto>
                    {
                        _mapper.Map<VehicleDto>(vehicle),
                    },
            }
        };
        var producer = _producerFactory.GetProducer<string, DataSynchronizationDto, JsonSerializer<DataSynchronizationDto>>();
        await producer.ProduceAsync(_kafkaConfig.Topic, JsonSerializer.Serialize(new { id = vehicle.Id.ToString() }), dataSync);
    }
}
