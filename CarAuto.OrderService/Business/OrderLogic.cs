using OrderProto = CarAuto.Protos.Order.Order;
using OrderEntity = CarAuto.OrderService.DAL.Entities.Order;
using AutoMapper;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.OrderService.Business.Interfaces;
using CarAuto.Kafka;
using CarAuto.Kafka.Json;
using CarAuto.Kafka.Config;
using Microsoft.EntityFrameworkCore;
using CarAuto.ClaimParser;

namespace CarAuto.OrderService.Business
{
    public class OrderLogic : IOrderLogic
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISequenceLogic _sequenceLogic;
        private readonly IProducerFactory _producerFactory;
        private readonly IClaimParser _claimParser;
        private readonly KafkaConfig _kafkaConfig;

        public OrderLogic(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISequenceLogic sequenceLogic,
            IProducerFactory producerFactory,
            IClaimParser claimParser,
            KafkaConfig kafkaConfig)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequenceLogic = sequenceLogic ?? throw new ArgumentNullException(nameof(sequenceLogic));
            _producerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
            _claimParser = claimParser ?? throw new ArgumentNullException(nameof(claimParser));
            _kafkaConfig = kafkaConfig ?? throw new ArgumentNullException(nameof(kafkaConfig));
        }

        public async Task<Guid> CreateOrderAsync(OrderProto orderProto)
        {
            var order = _mapper.Map<OrderEntity>(orderProto);
            _unitOfWork.GetRepository<OrderEntity>().Insert(order);
            order.OrderNumber = await _sequenceLogic.GetNextNoAsync();
            await _unitOfWork.SaveChangesAsync();
            await SendKafkaUpdateAsync(order);
            return order.Id;
        }

        public async Task DeleteOrderAsync(string id)
        {
            var orderId = Guid.Parse(id);
            var orderRepo = _unitOfWork.GetRepository<OrderEntity>();
            await orderRepo.DeleteAsync(orderId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<OrderProto>> GetAllOrdersAsync()
        {
            var orderRepo = _unitOfWork.GetRepository<OrderEntity>();
            return await orderRepo.Queryable.Include(e => e.Options).Select(e => _mapper.Map<OrderProto>(e)).ToListAsync();
        }

        public async Task<List<OrderProto>> GetUserOrdersAsync()
        {
            var userId = _claimParser.GetUserId();
            var orderRepo = _unitOfWork.GetRepository<OrderEntity>();
            return await orderRepo.Queryable.Include(e => e.Options).Where(e => e.CreatedBy == userId).Select(e => _mapper.Map<OrderProto>(e)).ToListAsync();
        }

        public async Task UpdateOrderAsync(OrderProto orderProto)
        {
            var orderRepo = await _unitOfWork.GetRepositoryAsync<OrderEntity>();
            var order = await orderRepo.GetByIdAsync(Guid.Parse(orderProto.Id));
            order = _mapper.Map(orderProto, order);
            await SendKafkaUpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }


        private async Task SendKafkaUpdateAsync(OrderEntity order)
        {
            var kafkaProducer = _producerFactory.GetProducer<string, OrderEntity, JsonSerializer<OrderEntity>>();
            await kafkaProducer.ProduceAsync(_kafkaConfig.Topic, order.Id.ToString(), order);
        }
    }
}
