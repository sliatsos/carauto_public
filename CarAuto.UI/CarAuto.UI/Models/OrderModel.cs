using CarAuto.Protos.Customer;
using CarAuto.Protos.Enums;
using CarAuto.Protos.Vehicle;

namespace CarAuto.UI.Models;

public class OrderModel
{
    public string OrderNumber { get; set; } = string.Empty;

    public OrderStatus OrderStatus { get; set; }

    public Customer Customer { get; set; } = new Customer(); 

    public Model Model { get; set; } = new Model();

    public List<Option> Options { get; set; } = new List<Option>();

    public string Id { get; set; } = string.Empty;

    public int ActiveIndex { get; set; }

    public double TotalCost
    {
        get => Options.Select(e => e.UnitCost).Sum();
    }

    public double TotalPrice
    {
        get => Options.Select(e => e.UnitPrice).Sum();
    }
}
