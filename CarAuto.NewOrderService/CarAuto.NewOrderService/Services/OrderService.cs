using CarAuto.OrderService.Business.Interfaces;
using CarAuto.Protos.Order;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static CarAuto.Protos.Order.OrderService;

namespace CarAuto.OrderService.Services;

public class OrderService : OrderServiceBase
{
    private readonly IOrderLogic _orderLogic;

    public OrderService(IOrderLogic orderLogic)
    {
        _orderLogic = orderLogic ?? throw new ArgumentNullException(nameof(orderLogic));
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var orderId = await _orderLogic.CreateOrderAsync(request.Order);
        return new CreateOrderResponse
        {
            Id = orderId.ToString(),
        };
    }

    public override async Task<Empty> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
    {
        await _orderLogic.DeleteOrderAsync(request.Id);
        return new Empty();
    }

    public override async Task<GetAllOrdersResponse> GetAllOrders(Empty request, ServerCallContext context)
    {
        var result = await _orderLogic.GetAllOrdersAsync();
        var response = new GetAllOrdersResponse();
        response.Orders.AddRange(result);
        return response;
    }

    public override async Task<GetUserOrdersResponse> GetUserOrders(Empty request, ServerCallContext context)
    {
        var result = await _orderLogic.GetUserOrdersAsync();
        var response = new GetUserOrdersResponse();
        response.Orders.AddRange(result);
        return response;
    }

    public override async Task<Empty> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
    {
        await _orderLogic.UpdateOrderAsync(request.Order);
        return new Empty();
    }
}
