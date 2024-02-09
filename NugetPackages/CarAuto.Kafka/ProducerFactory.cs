using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace CarAuto.Kafka
{
    /// <summary>
    /// Implementation of a PubSub ProducerFactory for Kafka.
    /// </summary>
    public class ProducerFactory : IProducerFactory
    {
        private readonly ProducerConfig _producerConfig;
        private readonly ILogger<ProducerFactory> _logger;
        private Handle _handle;

        /// <summary>
        /// Initializes a new instance of the ProducerFactory class.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance, that is used for establishing the connection with the Kafka broker.</param>
        /// <param name="logger">The ILogger instance, that is used for logging. </param>
        /// <param name="userContextAccessor">The User Context Accessor.</param>
        public ProducerFactory(IConfiguration configuration, ILogger<ProducerFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _producerConfig = new ProducerConfig();

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Bind("Kafka", _producerConfig);
        }

        /// <inheritdoc cref="IProducerFactory.GetProducer{TKey,TValue}"/>
        public IProducer<TKey, TValue, TSerializer> GetProducer<TKey, TValue, TSerializer>()
            where TValue : class, new()
            where TSerializer : ISerializer<TValue>, new()
        {
            if (_handle == null)
            {
                var producer = new Producer<TKey, TValue, TSerializer>(_producerConfig, _logger);
                _handle = producer.Handle;
                return producer;
            }

            return new Producer<TKey, TValue, TSerializer>(_handle, _logger);
        }
    }
}