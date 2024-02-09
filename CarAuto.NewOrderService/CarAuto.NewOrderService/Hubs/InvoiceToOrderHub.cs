using CarAuto.Protos.Order;
using Microsoft.AspNetCore.SignalR;

namespace CarAuto.NewOrderService.Hubs;

public class InvoiceToOrderHub : Hub<Order>
{
}
