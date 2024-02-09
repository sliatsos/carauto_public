using Confluent.Kafka;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CarAuto.Kafka
{
    /// <summary>
    /// Implementation of a PubSub producer for Kafka.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the message.</typeparam>
    /// <typeparam name="TValue">The type of the value of the message.</typeparam>
    /// <typeparam name="TSerializer">The type of the Serializer.</typeparam>
    public class Producer<TKey, TValue, TSerializer> : IProducer<TKey, TValue, TSerializer>
        where TValue : class, new()
        where TSerializer : ISerializer<TValue>, new()
    {
        private readonly ILogger<ProducerFactory> _logger;
        private readonly IProducer<TKey, TValue> _producer;

        internal Producer(Handle handle, ILogger<ProducerFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _producer = new DependentProducerBuilder<TKey, TValue>(handle)
                .SetValueSerializer(new TSerializer())
                .Build();
            Handle = _producer.Handle;
        }

        internal Producer(ProducerConfig producerConfig, ILogger<ProducerFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _producer = new ProducerBuilder<TKey, TValue>(producerConfig)
                .SetValueSerializer(new TSerializer())
                .Build();
            Handle = _producer.Handle;
        }

        internal Producer(IProducer<TKey, TValue> producer, ILogger<ProducerFactory> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        /// <summary>
        /// The Handle of the connection to Kafka.
        /// </summary>
        public Handle Handle { get; }

        /// <summary>
        /// Produces a new message to Kafka.
        /// </summary>
        /// <param name="topic">The topic to produce to.</param>
        /// <param name="key">The key of the message.</param>
        /// <param name="value">The value of the message.</param>
        /// <param name="headers">The headers to be added to the message.</param>
        /// <returns></returns>
        public async Task ProduceAsync(string topic, TKey key, TValue value, IDictionary<string, string> headers = null)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException(nameof(topic));
            }

            var message = new Message<TKey, TValue>
            {
                Key = key,
                Value = value,
            };

            message.Headers = new Headers();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    message.Headers.Add(new Header(header.Key, Encoding.UTF8.GetBytes(header.Value)));
                }
            }

            try
            {
                await _producer.ProduceAsync(topic, message);
            }
            catch (ProduceException<TKey, TValue> produceException)
            {
                _logger.LogError(produceException, produceException.Message);
                throw;
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogError(argumentException, argumentException.Message);
                throw;
            }
        }
    }
}