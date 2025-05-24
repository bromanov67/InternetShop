using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Order.CreateOrder
{
    public record CreateOrderCommand(Guid UserId, int PaymentTypeId, List<OrderItemDto> Items) : IRequest<Result<OrderDto>>;
}
