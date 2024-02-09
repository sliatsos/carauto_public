using Confluent.Kafka;
using Google.Protobuf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarAuto.Kafka
{
    /// <summary>
    /// Generic interface for a PubSub Producer.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the message.</typeparam>
    /// <typeparam name="TValue">The type of the value of the message.</typeparam>
    /// <typeparam name="TSerializer">The type of the Serializer.</typeparam>
    public interface IProducer<TKey, TValue, TSerializer>
        where TValue : class, new()
        where TSerializer : ISerializer<TValue>, new()
    {
        /// <summary>
        /// Produces a new message to a PubSub system.
        /// </summary>
        /// <param name="topic">The topic to produce to.</param>
        /// <param name="key">The key of the message.</param>
        /// <param name="value">The value of the message.</param>
        /// <param name="headers">The headers to be added to the message.</param>
        Task ProduceAsync(string topic, TKey key, TValue value, IDictionary<string, string> headers = null);
    }
}