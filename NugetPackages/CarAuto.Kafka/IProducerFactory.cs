using Confluent.Kafka;
using Google.Protobuf;

namespace CarAuto.Kafka
{
    /// <summary>
    /// Interface for a PubSub ProducerFactory.
    /// </summary>
    public interface IProducerFactory
    {
        /// <summary>
        /// Gets a Producer for the given Key and Value types.
        /// </summary>
        /// <typeparam name="TKey">The type of the key of the message.</typeparam>
        /// <typeparam name="TValue">The type of the value of the message.</typeparam>
        /// <returns>The Producer.</returns>
        IProducer<TKey, TValue, TSerializer> GetProducer<TKey, TValue, TSerializer>()
            where TValue : class, new()
            where TSerializer : ISerializer<TValue>, new();
    }
}