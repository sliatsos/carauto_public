using System.Text.Json.Serialization;

namespace CarAuto.UserService.DAL.DTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Action
{
    Insert,
    Update,
    Delete
}
