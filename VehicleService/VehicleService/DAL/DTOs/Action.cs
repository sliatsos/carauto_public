using System.Text.Json.Serialization;

namespace CarAuto.VehicleService.DAL.DTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Action
{
    Insert,
    Update,
    Delete
}
