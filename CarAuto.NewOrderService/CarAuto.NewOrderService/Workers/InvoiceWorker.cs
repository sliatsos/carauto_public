using CarAuto.Kafka.Json;
using CarAuto.Kafka;
using CarAuto.NewOrderService.DAL.DTOs;
using CarAuto.OrderService.Business.Interfaces;
using Confluent.Kafka;

namespace CarAuto.NewOrderService.Workers
{
    public class InvoiceWorker : BackgroundConsumer<string, InvoiceToOrderDto, JsonDeserializer<InvoiceToOrderDto>>
    {
        public InvoiceWorker(
            ILogger<BackgroundConsumer<string, InvoiceToOrderDto, JsonDeserializer<InvoiceToOrderDto>>> logger,
            IConfiguration configuration,
            IHostApplicationLifetime hostApplicationLifetime,
            IServiceScopeFactory serviceScopeFactory) : base(logger, configuration, hostApplicationLifetime, serviceScopeFactory)
        {
        }

        public override string Topics => "dev.general.orders.json";

        protected override async Task ProcessMessage(ConsumeResult<string, InvoiceToOrderDto> consumeResult, IServiceScope scope)
        {
            var orderLogic = scope.ServiceProvider.GetRequiredService<IOrderLogic>();
            await orderLogic.CreateInvoiceToOrderLinkAsync(consumeResult.Message.Value);
        }
    }
}
