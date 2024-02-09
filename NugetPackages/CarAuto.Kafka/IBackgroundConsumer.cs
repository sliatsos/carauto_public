using Microsoft.Extensions.Hosting;

namespace CarAuto.Kafka
{
    /// <summary>
    /// Generic interface for a Background Consumer.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the Kafka message.</typeparam>
    /// <typeparam name="TValue">The type of the value of the Kafka message.</typeparam>
    /// <typeparam name="TDeserializer">The type of the value deserializer for the Kafka message.</typeparam>
    public interface IBackgroundConsumer<TKey, TValue, TDeserializer> : IHostedService
        where TValue : class, new()
    {
    }
}