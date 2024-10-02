using CarAuto.Protos.Order;

namespace CarAuto.OrderService.Business.Interfaces
{
    public interface IOrderLogic
    {
        Task<Guid> CreateOrderAsync(Order orderProto);
        Task DeleteOrderAsync(string id);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetUserOrdersAsync();
        Task UpdateOrderAsync(Order orderProto);
    }
}