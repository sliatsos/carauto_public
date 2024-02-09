using AutoMapper;
using CarAuto.ClaimParser;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Kafka;
using CarAuto.Kafka.Config;
using CarAuto.Kafka.Json;
using CarAuto.Protos.Customer;
using CarAuto.UserService.Business.Interfaces;
using CarAuto.UserService.DAL.DTOs;
using CarAuto.UserService.DAL.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Action = CarAuto.UserService.DAL.DTOs.Action;
using CustomerEntity = CarAuto.UserService.DAL.Entities.Customer;
using CustomerProto = CarAuto.Protos.Customer.Customer;

namespace CarAuto.UserService.Business
{
    public class CustomerLogic : ICustomerLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProducerFactory _producerFactory;
        private readonly KafkaConfig _kafkaConfig;
        private readonly IClaimParser _claimParser;

        public CustomerLogic(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IProducerFactory producerFactory,
            IClaimParser claimParser,
            KafkaConfig kafkaConfig)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _producerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
            _kafkaConfig = kafkaConfig ?? throw new ArgumentNullException(nameof(kafkaConfig));
            _claimParser = claimParser ?? throw new ArgumentNullException(nameof(claimParser));
        }

        public async Task<GetAllCustomersResponse> GetAllCustomersAsync()
        {
            var customerRepo = await _unitOfWork.GetRepositoryAsync<CustomerEntity>();

            var result = new GetAllCustomersResponse();
            result.Customers.AddRange(customerRepo.Queryable.Select(e => _mapper.Map<CustomerProto>(e)));
            return result;
        }

        public async Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest customerRequest)
        {
            var customer = _mapper.Map<CustomerEntity>(customerRequest.Customer);
            var customerRepo = await _unitOfWork.GetRepositoryAsync<CustomerEntity>();
            await customerRepo.InsertAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            await SendCustomerStatusAsync(customer, Action.Insert);


            return new CreateCustomerResponse
            {
                Id = customer.Id.ToString(),
            };
        }

        public async Task<GetCustomerResponse> GetCustomerAsync(GetCustomerRequest request)
        {
            var customerRepo = await _unitOfWork.GetRepositoryAsync<CustomerEntity>();
            var customer = await customerRepo.GetByIdAsync(Guid.Parse(request.Id));

            return new GetCustomerResponse
            {
                Customer = _mapper.Map<CustomerProto>(customer),
            };
        }

        public async Task UpdateCustomerAsync(UpdateCustomerRequest request)
        {
            var customerRepo = await _unitOfWork.GetRepositoryAsync<CustomerEntity>();
            var customer = await customerRepo.GetByIdAsync(Guid.Parse(request.Customer.Id));
            customer = _mapper.Map(request.Customer, customer);
            await SendCustomerStatusAsync(customer, Action.Update);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(DeleteCustomerRequest request)
        {
            var customerRepo = await _unitOfWork.GetRepositoryAsync<CustomerEntity>();
            var customer = await customerRepo.GetByIdAsync(Guid.Parse(request.Id));
            await SendCustomerStatusAsync(customer, Action.Delete);
            customerRepo.Delete(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task SendCustomerStatusAsync(CustomerEntity customer, Action action)
        {
            var dataSync = new DataSynchronizationDto
            {
                Action = action,
                Entity = nameof(PayloadDto.Customer),
                Payload = new PayloadDto
                {
                    Customer = new List<CustomerDto>
                    {
                        _mapper.Map<CustomerDto>(customer),
                    },
                }
            };
            var producer = _producerFactory.GetProducer<string, DataSynchronizationDto, JsonSerializer<DataSynchronizationDto>>();
            await producer.ProduceAsync(_kafkaConfig.Topic, System.Text.Json.JsonSerializer.Serialize(new {id = customer.Id.ToString()}), dataSync);
        }

        public async Task<GetCustomerResponse> GetCurrentCustomer()
        {
            var userId = _claimParser.GetUserId();
            var customerRepo = await _unitOfWork.GetRepositoryAsync<CustomerEntity>();
            var currentCustomer = await customerRepo.Queryable.FirstOrDefaultAsync(e => e.UserId == userId);
            if (currentCustomer == null)
            {
                var userRepo = await _unitOfWork.GetRepositoryAsync<User>();
                var user = await userRepo.GetByIdAsync(userId);
                var customerResult = await CreateCustomerAsync(new CreateCustomerRequest
                {
                    Customer = new CustomerProto
                    {
                        DisplayName = $"{user.FirstName}, {user.LastName}",
                        Phone = user.PhoneNumber,
                        Email = user.Email,
                    }
                });

                currentCustomer = await customerRepo.GetByIdAsync(Guid.Parse(customerResult.Id));
            }

            return new GetCustomerResponse
            {
                Customer = _mapper.Map<CustomerProto>(currentCustomer),
            };
        }
    }
}
