
namespace InternetShop.Application.BusinessLogic.Order.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken);
        Task<List<Order>> GetOrderByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Guid> CreateOrderAsync(Order order, CancellationToken cancellationToken);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken);
        Task<bool> AddOrderItemAsync(Guid orderId, OrderItem item, CancellationToken cancellationToken);
        Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken);
    }
}
