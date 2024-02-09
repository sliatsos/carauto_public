using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CarAuto.Kafka.Json
{
    /// <summary>
    /// Implements a Protobuf deserializer for use with a Kafka Consumer.
    /// </summary>
    /// <typeparam name="T">The type of the Protobuf message to be deserialized to.</typeparam>
    public class JsonDeserializer<T> : IDeserializer<T>
            where T : class, new()
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters =
                {
                    new JsonStringEnumConverter(),
                }
        };

        /// <summary>
        /// Deserializes data to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="data">the data to be deserialized.</param>
        /// <param name="isNull">specifies whether data is null.</param>
        /// <param name="context">The SerializationContext under which the serialization happens including the Kafka topic and the collection of headers.</param>
        /// <returns>the deserialized data.</returns>
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(data, _options);
        }
    }
}