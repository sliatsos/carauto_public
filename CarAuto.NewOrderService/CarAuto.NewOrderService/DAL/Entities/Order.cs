using CarAuto.EFCore.BaseEntity;
using CarAuto.Protos.Enums;

namespace CarAuto.OrderService.DAL.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public Guid ModelId { get; set; }

    public Guid VehicleId { get; set; }

    public List<Option> Options { get; set; } = new List<Option>();

    public OrderStatus OrderStatus { get; set; }

    public string Pdf { get; set; } = string.Empty;
}
