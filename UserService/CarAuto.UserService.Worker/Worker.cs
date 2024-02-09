using CarAuto.Kafka;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Kafka.Config;
using CarAuto.Kafka.Json;
using CarAuto.UserService.DAL.DTOs;
using CarAuto.UserService.DAL.Entities;
using CarAuto.UserService.Worker.BusinessLogic;
using Confluent.Kafka;
using User = CarAuto.Protos.User.User;

namespace CarAuto.UserService.Worker;

public class Worker : BackgroundConsumer<string, DataSynchronizationDto, JsonDeserializer<DataSynchronizationDto>>
{
    private readonly KafkaConfig _kafkaConfig;

    public Worker(
        ILogger<BackgroundConsumer<string, DataSynchronizationDto, JsonDeserializer<DataSynchronizationDto>>> logger,
        IConfiguration configuration,
        IHostApplicationLifetime hostApplicationLifetime,
        IServiceScopeFactory serviceScopeFactory,
        KafkaConfig kafkaConfig) 
        : base(logger, configuration, hostApplicationLifetime, serviceScopeFactory)
    {
        _kafkaConfig = kafkaConfig ?? throw new ArgumentNullException(nameof(kafkaConfig));
    }

    public override string Topics => _kafkaConfig.Topic;

    protected override async Task ProcessMessage(ConsumeResult<string, DataSynchronizationDto> consumeResult, IServiceScope scope)
    {
        var syncLogic = scope.ServiceProvider.GetRequiredService<ISyncLogic>();

        await syncLogic.SyncChangesAsync(consumeResult.Message.Value);
    }
}
