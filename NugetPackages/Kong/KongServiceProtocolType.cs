using System.Runtime.Serialization;

namespace CarAuto.Kong;

public enum KongServiceProtocolType
{
    [EnumMember(Value = "http")]
    Http,

    [EnumMember(Value = "https")]
    Https,

    [EnumMember(Value = "grpc")]
    Grpc,

    [EnumMember(Value = "grpcs")]
    Grpcs,
}
