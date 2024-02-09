using CarAuto.Kafka;
using CarAuto.Kafka.Json;
using CarAuto.NewOrderService.DAL.DTOs;
using CarAuto.OrderService.Business.Interfaces;
using Confluent.Kafka;

namespace CarAuto.NewOrderService.Workers
{
    public class OrderWorker : BackgroundConsumer<string, QuoteToOrderDto, JsonDeserializer<QuoteToOrderDto>>
    {
        public OrderWorker(
            ILogger<BackgroundConsumer<string, QuoteToOrderDto, JsonDeserializer<QuoteToOrderDto>>> logger,
            IConfiguration configuration,
            IHostApplicationLifetime hostApplicationLifetime,
            IServiceScopeFactory serviceScopeFactory) : base(logger, configuration, hostApplicationLifetime, serviceScopeFactory)
        {
        }

        public override string Topics => "dev.general.orders.json";

        protected override async Task ProcessMessage(ConsumeResult<string, QuoteToOrderDto> consumeResult, IServiceScope scope)
        {
            var orderLogic = scope.ServiceProvider.GetRequiredService<IOrderLogic>();
            await orderLogic.CreateQuoteToOrderLinkAsync(consumeResult.Message.Value);
        }
    }
}
