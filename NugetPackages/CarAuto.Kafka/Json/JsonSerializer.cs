using Confluent.Kafka;

namespace CarAuto.Kafka.Json
{
    /// <summary>
    /// Implements a Protobuf serializer for use with a Kafka Producer.
    /// </summary>
    /// <typeparam name="T">The type of the Protobuf message to be serialized.</typeparam>
    public class JsonSerializer<T> : ISerializer<T>
        where T : class
    {
        /// <summary>
        /// Serializes data to a byte[]
        /// </summary>
        /// <param name="data">The data to be serialized.</param>
        /// <param name="context">The SerializationContext under which the serialization happens including the Kafka topic and the collection of headers.</param>
        /// <returns>the serialized data as byte[]</returns>
        public byte[] Serialize(T data, SerializationContext context)
        {
            return data != null ? System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data) : null;
        }
    }
}