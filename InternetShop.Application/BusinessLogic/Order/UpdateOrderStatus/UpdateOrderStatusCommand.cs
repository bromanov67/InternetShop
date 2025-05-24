using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Order.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest<Result<OrderDto>>;
}
