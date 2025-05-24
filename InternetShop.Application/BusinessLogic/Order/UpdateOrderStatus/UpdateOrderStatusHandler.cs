using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using InternetShop.Application.BusinessLogic.Order.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InternetShop.Application.BusinessLogic.Order.UpdateOrderStatus
{
    public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UpdateOrderStatusHandler> _logger;

        public UpdateOrderStatusHandler(
            IOrderRepository orderRepository,
            ILogger<UpdateOrderStatusHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(
            UpdateOrderStatusCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
                if (order == null)
                    return Result.Fail<OrderDto>("Order not found");

                var success = await _orderRepository.UpdateOrderStatusAsync(
                    request.OrderId,
                    request.NewStatus,
                    cancellationToken);

                if (!success)
                    return Result.Fail<OrderDto>("Failed to update order status");

                var updatedOrder = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
                return updatedOrder == null
                    ? Result.Fail<OrderDto>("Order not found after update")
                    : Result.Ok(MapToDto(updatedOrder));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderId}", request.OrderId);
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
