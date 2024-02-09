using AutoMapper;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Kafka.Config;
using CarAuto.Kafka;
using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.Business.Interfaces;
using CarAuto.Kafka.Json;
using CarAuto.VehicleService.DAL.DTOs;
using BrandEntity = CarAuto.VehicleService.DAL.Entities.Brand;
using BrandProto = CarAuto.Protos.Vehicle.Brand;
using Action = CarAuto.VehicleService.DAL.DTOs.Action;

namespace CarAuto.VehicleService.Business;

public class BrandLogic : IBrandLogic
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProducerFactory _producerFactory;
    private readonly KafkaConfig _kafkaConfig;

    public BrandLogic(
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

    public async Task<CreateBrandResponse> CreateBrandAsync(CreateBrandRequest request)
    {
        var brand = _mapper.Map<BrandEntity>(request.Brand);
        var brandRepo = await _unitOfWork.GetRepositoryAsync<BrandEntity>();
        await brandRepo.InsertAsync(brand);
        await _unitOfWork.SaveChangesAsync();

        await SendBrandStatusAsync(brand, Action.Insert);


        return new CreateBrandResponse
        {
            Id = brand.Id.ToString(),
        };
    }

    public async Task DeleteBrandAsync(DeleteBrandRequest request)
    {
        var brandRepo = await _unitOfWork.GetRepositoryAsync<BrandEntity>();
        var brand = await brandRepo.GetByIdAsync(Guid.Parse(request.Id));
        await SendBrandStatusAsync(brand, Action.Delete);
        brandRepo.Delete(brand);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GetAllBrandsResponse> GetAllBrandsAsync()
    {
        var brandRepo = await _unitOfWork.GetRepositoryAsync<BrandEntity>();

        var result = new GetAllBrandsResponse();
        result.Brands.AddRange(brandRepo.Queryable.Select(e => _mapper.Map<BrandProto>(e)));
        return result;
    }

    public async Task<GetBrandResponse> GetBrandAsync(GetBrandRequest request)
    {
        var brandRepo = await _unitOfWork.GetRepositoryAsync<BrandEntity>();
        var brand = brandRepo.Queryable
            .FirstOrDefault(e => e.Id == Guid.Parse(request.Id));

        return new GetBrandResponse
        {
            Brand = _mapper.Map<BrandProto>(brand),
        };
    }

    public async Task UpdateBrandAsync(UpdateBrandRequest request)
    {
        var vehicleRepo = await _unitOfWork.GetRepositoryAsync<BrandEntity>();
        var vehicle = await vehicleRepo.GetByIdAsync(Guid.Parse(request.Brand.Id));
        vehicle = _mapper.Map(request.Brand, vehicle);
        await SendBrandStatusAsync(vehicle, Action.Update);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SendBrandStatusAsync(BrandEntity vehicle, Action action)
    {
        var dataSync = new DataSynchronizationDto
        {
            Action = action,
            Entity = nameof(PayloadDto.Brands),
            Payload = new PayloadDto
            {
                Brands = new List<BrandDto>
                {
                    _mapper.Map<BrandDto>(vehicle),
                },
            }
        };
        var producer = _producerFactory.GetProducer<string, DataSynchronizationDto, JsonSerializer<DataSynchronizationDto>>();
        await producer.ProduceAsync(_kafkaConfig.Topic, System.Text.Json.JsonSerializer.Serialize(new {id = vehicle.Id.ToString() }), dataSync);
    }
}
