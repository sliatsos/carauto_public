using AutoMapper;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Kafka;
using CarAuto.Kafka.Config;
using CarAuto.Kafka.Json;
using CarAuto.Protos.Customer;
using CarAuto.Protos.Salesperson;
using CarAuto.UserService.Business.Interfaces;
using CarAuto.UserService.DAL.DTOs;
using CarAuto.UserService.DAL.Entities;
using Action = CarAuto.UserService.DAL.DTOs.Action;
using Salesperson = CarAuto.UserService.DAL.Entities.Salesperson;
using SalespersonProto = CarAuto.Protos.Salesperson.Salesperson;

namespace CarAuto.UserService.Business
{
    public class SalespersonLogic : ISalespersonLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProducerFactory _producerFactory;
        private readonly KafkaConfig _kafkaConfig;

        public SalespersonLogic(IUnitOfWork unitOfWork, IMapper mapper, IProducerFactory producerFactory, KafkaConfig kafkaConfig)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _producerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
            _kafkaConfig = kafkaConfig ?? throw new ArgumentNullException(nameof(kafkaConfig));
        }

        public async Task<GetAllSalespersonResponse> GetAllSalespersonsAsync()
        {
            var salespersonRepo = await _unitOfWork.GetRepositoryAsync<Salesperson>();
            var salespersons = salespersonRepo.Queryable.Select(e => _mapper.Map<SalespersonProto>(e));
            var result = new GetAllSalespersonResponse();
            result.Salesperson.AddRange(salespersons);

            return result;
        }

        public async Task<GetSalespersonResponse> GetSalespersonAsync(GetSalespersonRequest request)
        {
            var salespersonRepo = await _unitOfWork.GetRepositoryAsync<Salesperson>();
            var salesperson = await salespersonRepo.GetByIdAsync(Guid.Parse(request.Id));

            return new GetSalespersonResponse
            {
                Salesperson = _mapper.Map<SalespersonProto>(salesperson),
            };
        }

        public async Task<CreateSalespersonResponse> CreateSalespersonAsync(CreateSalespersonRequest salespersonRequest)
        {
            var salesperson = _mapper.Map<Salesperson>(salespersonRequest.Salesperson);
            var salespersonRepo = await _unitOfWork.GetRepositoryAsync<Salesperson>();
            await salespersonRepo.InsertAsync(salesperson);
            await _unitOfWork.SaveChangesAsync();

            await SendSalespersonStatusAsync(salesperson, Action.Insert);


            return new CreateSalespersonResponse
            {
                Id = salesperson.Id.ToString(),
            };
        }

        public async Task UpdateSalespersonAsync(UpdateSalespersonRequest request)
        {
            var salespersonRepo = await _unitOfWork.GetRepositoryAsync<Salesperson>();
            var salesperson = await salespersonRepo.GetByIdAsync(Guid.Parse(request.Salesperson.Id));
            salesperson = _mapper.Map(request.Salesperson, salesperson);
            await SendSalespersonStatusAsync(salesperson, Action.Update);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSalespersonAsync(DeleteSalespersonRequest request)
        {
            var salespersonRepo = await _unitOfWork.GetRepositoryAsync<Salesperson>();
            var salesperson = await salespersonRepo.GetByIdAsync(Guid.Parse(request.Id));
            await SendSalespersonStatusAsync(salesperson, Action.Delete);
            salespersonRepo.Delete(salesperson);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task SendSalespersonStatusAsync(Salesperson salesperson, Action action)
        {
            var dataSync = new DataSynchronizationDto
            {
                Action = action,
                Entity = nameof(PayloadDto.Salesperson),
                Payload = new PayloadDto
                {
                    Salesperson = new List<SalespersonDto>
                    {
                        _mapper.Map<SalespersonDto>(salesperson),
                    },
                }
            };
            var producer = _producerFactory.GetProducer<string, DataSynchronizationDto, JsonSerializer<DataSynchronizationDto>>();
            await producer.ProduceAsync(_kafkaConfig.Topic, System.Text.Json.JsonSerializer.Serialize(new { id = salesperson.Id.ToString() }), dataSync);
        }
    }
}
