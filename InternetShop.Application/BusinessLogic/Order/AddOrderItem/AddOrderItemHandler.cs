using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using InternetShop.Application.BusinessLogic.Order.Interfaces;
using InternetShop.Application.BusinessLogic.Product;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InternetShop.Application.BusinessLogic.Order.AddOrderItem
{
    public class AddOrderItemHandler : IRequestHandler<AddOrderItemCommand, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICatalogRepository _productRepository;
        private readonly ILogger<AddOrderItemHandler> _logger;

        public AddOrderItemHandler(
            IOrderRepository orderRepository,
            ICatalogRepository productRepository,
            ILogger<AddOrderItemHandler> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(
            AddOrderItemCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Проверяем существование продукта
                var product = await _productRepository.GetProductByIdAsync(request.Item.ProductId, cancellationToken);
                if (product == null)
                {
                    return Result.Fail<OrderDto>($"Product with ID {request.Item.ProductId} not found");
                }

                // 2. Создаем OrderItem
                var orderItem = new OrderItem(
                    request.Item.ProductId,
                    product.Name,
                    product.Price,
                    request.Item.Quantity);

                // 3. Добавляем в заказ
                var success = await _orderRepository.AddOrderItemAsync(
                    request.OrderId,
                    orderItem,
                    cancellationToken);

                if (!success)
                {
                    return Result.Fail<OrderDto>("Failed to add item to order");
                }

                // 4. Возвращаем обновленный заказ
                var updatedOrder = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
                return updatedOrder == null
                    ? Result.Fail<OrderDto>("Order not found after update")
                    : Result.Ok(MapToDto(updatedOrder));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to order {OrderId}", request.OrderId);
                return Result.Fail<OrderDto>("Internal server error");
            }
        }

        private OrderDto MapToDto(Order order) => new(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.Status,
            order.TotalAmount,
                order.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.Quantity)).ToList());
    }
}
