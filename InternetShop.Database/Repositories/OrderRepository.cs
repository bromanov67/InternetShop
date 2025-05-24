using InternetShop.Application.BusinessLogic.Order;
using InternetShop.Application.BusinessLogic.Order.DTO;
using InternetShop.Application.BusinessLogic.Order.Interfaces;
using InternetShop.Database.DbContext;
using InternetShop.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Database.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly UserDbContext _context;

        public OrderRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .Include(o => o.Status)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);

            return entities == null ? null : MapToDomain(entities);
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var entity = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            return entity == null ? null : MapToDomain(entity);
        }

        public async Task<List<Order>> GetOrderByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var entities = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .ToListAsync(cancellationToken);

            return entities.Select(MapToDomain).ToList();
        }

        public async Task<Guid> CreateOrderAsync(Order order, CancellationToken cancellationToken)
        {
            // Получаем начальный статус "Created" из БД
            var status = await _context.OrderStatuses
                .FirstOrDefaultAsync(s => s.Name == order.Status.ToString(), cancellationToken);

            if (status == null)
            {
                throw new InvalidOperationException("Order status not found");
            }

            var entity = new OrderEntity
            {
                Id = order.Id,
                UserId = order.UserId,
                StatusId = status.Id,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(i => new OrderItemEntity
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),

                // Создаем связанную запись о платеже
                Payment = new PaymentEntity
                {
                    Id = Guid.NewGuid(),
                    PaymentTypeId = order.PaymentTypeId,
                    Amount = order.TotalAmount,
                    Status = "Pending",
                    TransactionId = Guid.NewGuid().ToString(),
                    PaymentDate = DateTime.UtcNow
                }
            };

            await _context.Orders.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken)
        {
            var statusName = Enum.GetName(typeof(OrderStatus), newStatus);

            var status = await _context.OrderStatuses
                .FirstOrDefaultAsync(s => s.Name == statusName, cancellationToken);

            if (status == null)
            {
                return false;
            }

            var entity = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (entity == null)
                return false;

            entity.StatusId = status.Id;

            // Если заказ завершен, обновляем статус платежа
            if (newStatus == OrderStatus.Delivered)
            {
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

                if (payment != null)
                {
                    payment.Status = "Completed";
                }
            }

            _context.Orders.Update(entity);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var entity = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (entity == null) return false;

            _context.Orders.Remove(entity);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> AddOrderItemAsync(Guid orderId, OrderItem item, CancellationToken cancellationToken)
        {
            var orderEntity = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (orderEntity == null)
                return false;

            // Проверяем, что заказ в допустимом статусе для добавления товаров
            var currentStatus = orderEntity.Status?.Name;
            if (currentStatus != "Created" && currentStatus != "Pending")
                return false;

            var itemEntity = new OrderItemEntity
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price
            };

            orderEntity.Items.Add(itemEntity);
            orderEntity.TotalAmount += item.Price * item.Quantity;

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> ProcessPaymentAsync(Guid orderId, PaymentInfo payment, CancellationToken cancellationToken)
        {
            var orderEntity = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (orderEntity == null || orderEntity.Payment == null)
                return false;

            // Проверяем, что заказ в допустимом статусе для оплаты
            if (orderEntity.Status?.Name != "Pending")
                return false;

            orderEntity.Payment.Status = "Completed";
            orderEntity.Payment.TransactionId = payment.TransactionId;
            orderEntity.Payment.PaymentDate = DateTime.UtcNow;

            // Обновляем статус заказа
            var paidStatus = await _context.OrderStatuses
                .FirstOrDefaultAsync(s => s.Name == "Paid", cancellationToken);

            if (paidStatus != null)
            {
                orderEntity.StatusId = paidStatus.Id;
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var orderEntity = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (orderEntity == null)
                return false;

            // Проверяем, что заказ можно отменить
            var currentStatus = orderEntity.Status?.Name;
            if (currentStatus == "Completed" || currentStatus == "Cancelled")
                return false;

            var cancelledStatus = await _context.OrderStatuses
                .FirstOrDefaultAsync(s => s.Name == "Cancelled", cancellationToken);

            if (cancelledStatus == null)
                return false;

            orderEntity.StatusId = cancelledStatus.Id;

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken)
        {
            var statusName = status.ToString();
            var entities = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Status)
                .Where(o => o.Status.Name == statusName)
                .ToListAsync(cancellationToken);

            return entities.Select(MapToDomain).ToList();
        }
        private static Order MapToDomain(OrderEntity entity)
        {
            var statusName = entity.Status?.Name ?? "Created";

            var status = Enum.Parse<OrderStatus>(statusName);

            return new Order
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
                Status = status,
                TotalAmount = entity.TotalAmount,          
                Items = entity.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Price = i.Price,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        private static List<Order> MapToDomain(List<OrderEntity> entities)
        {
            if (entities == null) return null;
            return entities.Select(MapToDomain).ToList();
        }
    }
}