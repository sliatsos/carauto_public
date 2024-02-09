using AutoMapper;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Kafka.Config;
using CarAuto.Kafka;
using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.Business.Interfaces;
using CarAuto.Kafka.Json;
using CarAuto.VehicleService.DAL.DTOs;
using OptionEntity = CarAuto.VehicleService.DAL.Entities.Option;
using OptionProto = CarAuto.Protos.Vehicle.Option;
using Action = CarAuto.VehicleService.DAL.DTOs.Action;
using System.Text.Json;

namespace CarAuto.VehicleService.Business;

public class OptionLogic : IOptionLogic
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProducerFactory _producerFactory;
    private readonly KafkaConfig _kafkaConfig;

    public OptionLogic(
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

    public async Task<CreateOptionResponse> CreateOptionAsync(CreateOptionRequest request)
    {
        var Option = _mapper.Map<OptionEntity>(request.Option);
        var OptionRepo = await _unitOfWork.GetRepositoryAsync<OptionEntity>();
        await OptionRepo.InsertAsync(Option);
        await _unitOfWork.SaveChangesAsync();

        await SendOptionStatusAsync(Option, Action.Insert);

        return new CreateOptionResponse
        {
            Id = Option.Id.ToString(),
        };
    }

    public async Task DeleteOptionAsync(DeleteOptionRequest request)
    {
        var OptionRepo = await _unitOfWork.GetRepositoryAsync<OptionEntity>();
        var Option = await OptionRepo.GetByIdAsync(Guid.Parse(request.Id));
        await SendOptionStatusAsync(Option, Action.Delete);
        OptionRepo.Delete(Option);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GetAllOptionsResponse> GetAllOptionsAsync()
    {
        var OptionRepo = await _unitOfWork.GetRepositoryAsync<OptionEntity>();

        var result = new GetAllOptionsResponse();
        result.Options.AddRange(OptionRepo.Queryable.Select(e => _mapper.Map<OptionProto>(e)));
        return result;
    }

    public async Task<GetOptionResponse> GetOptionAsync(GetOptionRequest request)
    {
        var OptionRepo = await _unitOfWork.GetRepositoryAsync<OptionEntity>();
        var Option = OptionRepo.Queryable
            .FirstOrDefault(e => e.Id == Guid.Parse(request.Id));

        return new GetOptionResponse
        {
            Option = _mapper.Map<OptionProto>(Option),
        };
    }

    public async Task UpdateOptionAsync(UpdateOptionRequest request)
    {
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<OptionEntity>();
        var vehicle = await vehicleRepo.GetByIdAsync(Guid.Parse(request.Option.Id));
        vehicle = _mapper.Map(request.Option, vehicle);
        await SendOptionStatusAsync(vehicle, Action.Update);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SendOptionStatusAsync(OptionEntity option, Action action)
    {
        var dataSync = new DataSynchronizationDto
        {
            Action = action,
            Entity = nameof(PayloadDto.Options),
            Payload = new PayloadDto
            {
                Options = new List<OptionDto>
                    {
                        _mapper.Map<OptionDto>(option),
                    },
            }
        };
        var producer = _producerFactory.GetProducer<string, DataSynchronizationDto, JsonSerializer<DataSynchronizationDto>>();
        await producer.ProduceAsync(_kafkaConfig.Topic, JsonSerializer.Serialize(new { id = option.Id.ToString() }), dataSync);
    }
}
