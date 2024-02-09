using Confluent.Kafka;
using Google.Protobuf;
using System;

namespace CarAuto.Kafka.Proto
{
    /// <summary>
    /// Implements a Protobuf deserializer for use with a Kafka Consumer.
    /// </summary>
    /// <typeparam name="T">The type of the Protobuf message to be deserialized to.</typeparam>
    public class ProtoDeserializer<T> : IDeserializer<T>
            where T : IMessage<T>, new()
    {
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

            var instance = new T();

            instance.MergeFrom(data.ToArray());
            return instance;
        }
    }
}