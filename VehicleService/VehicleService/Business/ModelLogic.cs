using AutoMapper;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Kafka.Config;
using CarAuto.Kafka;
using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.Business.Interfaces;
using CarAuto.Kafka.Json;
using CarAuto.VehicleService.DAL.DTOs;
using ModelEntity = CarAuto.VehicleService.DAL.Entities.Model;
using ModelProto = CarAuto.Protos.Vehicle.Model;
using Action = CarAuto.VehicleService.DAL.DTOs.Action;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CarAuto.VehicleService.Business;

public class ModelLogic : IModelLogic
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProducerFactory _producerFactory;
    private readonly KafkaConfig _kafkaConfig;

    public ModelLogic(
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

    public async Task<CreateModelResponse> CreateModelAsync(CreateModelRequest request)
    {
        var Model = _mapper.Map<ModelEntity>(request.Model);
        var ModelRepo = await _unitOfWork.GetRepositoryAsync<ModelEntity>();
        await ModelRepo.InsertAsync(Model);
        await _unitOfWork.SaveChangesAsync();

        await SendModelStatusAsync(Model, Action.Insert);


        return new CreateModelResponse
        {
            Id = Model.Id.ToString(),
        };
    }

    public async Task DeleteModelAsync(DeleteModelRequest request)
    {
        var ModelRepo = await _unitOfWork.GetRepositoryAsync<ModelEntity>();
        var Model = await ModelRepo.GetByIdAsync(Guid.Parse(request.Id));
        await SendModelStatusAsync(Model, Action.Delete);
        ModelRepo.Delete(Model);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GetAllModelsResponse> GetAllModelsAsync()
    {
        var ModelRepo = await _unitOfWork.GetRepositoryAsync<ModelEntity>();

        var result = new GetAllModelsResponse();
        result.Models.AddRange(ModelRepo.Queryable.Include(e => e.Options).Select(e => _mapper.Map<ModelProto>(e)));
        return result;
    }

    public async Task<GetModelResponse> GetModelAsync(GetModelRequest request)
    {
        var ModelRepo = await _unitOfWork.GetRepositoryAsync<ModelEntity>();
        var Model = ModelRepo.Queryable
            .Include(e => e.Options)
            .FirstOrDefault(e => e.Id == Guid.Parse(request.Id));

        return new GetModelResponse
        {
            Model = _mapper.Map<ModelProto>(Model),
        };
    }

    public async Task UpdateModelAsync(UpdateModelRequest request)
    {
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<ModelEntity>();
        var vehicle = await vehicleRepo.GetByIdAsync(Guid.Parse(request.Model.Id));
        vehicle = _mapper.Map(request.Model, vehicle);
        await SendModelStatusAsync(vehicle, Action.Update);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SendModelStatusAsync(ModelEntity model, Action action)
    {
        var dataSync = new DataSynchronizationDto
        {
            Action = action,
            Entity = nameof(PayloadDto.Models),
            Payload = new PayloadDto
            {
                Models = new List<ModelDto>
                    {
                        _mapper.Map<ModelDto>(model),
                    },
            }
        };
        var producer = _producerFactory.GetProducer<string, DataSynchronizationDto, JsonSerializer<DataSynchronizationDto>>();
        await producer.ProduceAsync(_kafkaConfig.Topic, JsonSerializer.Serialize(new { id = model.Id.ToString() }), dataSync);
    }
}
