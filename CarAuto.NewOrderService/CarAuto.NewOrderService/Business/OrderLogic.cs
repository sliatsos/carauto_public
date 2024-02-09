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
using CarAuto.Protos.Salesperson;
using static Confluent.Kafka.ConfigPropertyNames;
using Microsoft.AspNetCore.SignalR;
using CarAuto.NewOrderService.DAL.DTOs;
using CarAuto.NewOrderService.DAL.Entities;
using CarAuto.NewOrderService.Hubs;
using CarAuto.Protos.Vehicle;
using Google.Protobuf.WellKnownTypes;
using CarAuto.VehicleService.DAL.DTOs;

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
        private readonly IHubContext<QuoteToOrderHub> _quoteToOrderHub;
        private readonly IHubContext<InvoiceToOrderHub> _invoiceToOrderHub;

        public OrderLogic(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISequenceLogic sequenceLogic,
            IProducerFactory producerFactory,
            IClaimParser claimParser,
            KafkaConfig kafkaConfig,
            IHubContext<QuoteToOrderHub> quoteToOrderHub,
            IHubContext<InvoiceToOrderHub> invoiceToOrderHub)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _sequenceLogic = sequenceLogic ?? throw new ArgumentNullException(nameof(sequenceLogic));
            _producerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
            _claimParser = claimParser ?? throw new ArgumentNullException(nameof(claimParser));
            _kafkaConfig = kafkaConfig ?? throw new ArgumentNullException(nameof(kafkaConfig));
            _quoteToOrderHub = quoteToOrderHub ?? throw new ArgumentNullException(nameof(quoteToOrderHub));
            _invoiceToOrderHub = invoiceToOrderHub ?? throw new ArgumentNullException(nameof(invoiceToOrderHub));
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

        public async Task CreateQuoteToOrderLinkAsync(QuoteToOrderDto quoteToOrder)
        {
            if (quoteToOrder.State != "Quote To Order")
            {
                return;
            }

            var repo = _unitOfWork.GetRepository<QuoteOrderLink>();

            var link = repo.Queryable.FirstOrDefault(e => e.QuoteId == quoteToOrder.Header.QuoteId);
            if (link == null)
            {
                link = new QuoteOrderLink
                {
                    OrderNumber = quoteToOrder.Header.OrderNumber,
                    OrderId = quoteToOrder.Header.OrderId,
                    QuoteId = quoteToOrder.Header.QuoteId,
                };

                repo.Insert(link);
                await repo.SaveChangesAsync();

                var orderRepo = _unitOfWork.GetRepository<OrderEntity>();
                var order = await orderRepo.GetByIdAsync(link.QuoteId);
                order.OrderStatus = Protos.Enums.OrderStatus.Approved;
                await _unitOfWork.SaveChangesAsync();

                await _quoteToOrderHub.Clients.All.SendAsync("QuoteToOrder", _mapper.Map<OrderProto>(order));
            }
        }

        public async Task CreateInvoiceToOrderLinkAsync(InvoiceToOrderDto invoiceToOrder)
        {
            if (invoiceToOrder.State != "Backend Invoiced")
            {
                return;
            }
            var quoteToOrderRepo = await _unitOfWork.GetRepositoryAsync<QuoteOrderLink>();
            var quoteOrderLink = quoteToOrderRepo.Queryable.FirstOrDefault(e => e.OrderId == invoiceToOrder.Header.OrderId);
            if (quoteOrderLink == null)
            {
                return;
            }

            var repo = _unitOfWork.GetRepository<OrderEntity>();
            var order = await repo.GetByIdAsync(quoteOrderLink.QuoteId);
            order.Pdf = invoiceToOrder.Header.Pdf;
            order.OrderStatus = Protos.Enums.OrderStatus.Invoiced;
            await _unitOfWork.SaveChangesAsync();
            var vehicle = invoiceToOrder.Header.Vehicle;
            var options = invoiceToOrder.Header.Lines;
            var kafkaProducer = _producerFactory.GetProducer<string, object, JsonSerializer<object>>();
            var vehicleToSend = new VehicleServiceDto
            {
                BrandId = vehicle.BrandId,
                BusinessPartnerId = vehicle.BusinessPartnerId,
                BusinessPartnerType = vehicle.BusinessPartnerType,
                Code = vehicle.Code,
                ExternalId = vehicle.ExternalId.ToString(),
                Id = vehicle.Id,
                LicenseNo = vehicle.LicenseNo,
                MileageUnitOfMeasure = vehicle.MileageUnitOfMeasure,
                ModelId = vehicle.ModelId,
                RegistrationDate = vehicle.RegistrationDate,
                VehicleStatus = vehicle.VehicleStatus,
                VehicleType = vehicle.VehicleType,
                Vin = vehicle.Vin,
                ModifiedOn = DateTime.UtcNow,
            };
            foreach(var option in options)
            {
                vehicleToSend.Options.Add(new OptionServiceDto
                {
                    Id = option.ProductId,
                });
            }

            await kafkaProducer.ProduceAsync("dev.general.vehicles.json", System.Text.Json.JsonSerializer.Serialize(
                new 
                { 
                    id = invoiceToOrder.Header.Vehicle.Id.ToString() 
                }), vehicleToSend);

            await _invoiceToOrderHub.Clients.All.SendAsync("InvoiceToOrder", _mapper.Map<OrderProto>(order));
        }


        private async Task SendKafkaUpdateAsync(OrderEntity order)
        {
            var kafkaProducer = _producerFactory.GetProducer<string, OrderEntity, JsonSerializer<OrderEntity>>();
            await kafkaProducer.ProduceAsync(_kafkaConfig.Topic, System.Text.Json.JsonSerializer.Serialize(new { id = order.Id.ToString() }), order);
        }
    }
}
