using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Order.AddOrderItem
{
    public record AddOrderItemCommand(Guid OrderId, AddOrderItemDto Item) : IRequest<Result<OrderDto>>;
}
