using Confluent.Kafka;
using Google.Protobuf;

namespace CarAuto.Kafka.Proto
{
    /// <summary>
    /// Implements a Protobuf serializer for use with a Kafka Producer.
    /// </summary>
    /// <typeparam name="T">The type of the Protobuf message to be serialized.</typeparam>
    public class ProtoSerializer<T> : ISerializer<T>
        where T : IMessage<T>
    {
        /// <summary>
        /// Serializes data to a byte[]
        /// </summary>
        /// <param name="data">The data to be serialized.</param>
        /// <param name="context">The SerializationContext under which the serialization happens including the Kafka topic and the collection of headers.</param>
        /// <returns>the serialized data as byte[]</returns>
        public byte[] Serialize(T data, SerializationContext context)
        {
            return data?.ToByteArray() ?? null;
        }
    }
}