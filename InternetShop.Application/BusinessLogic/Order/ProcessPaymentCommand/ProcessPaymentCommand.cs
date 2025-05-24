using FluentResults;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Order.ProcessPaymentCommand
{
    public record ProcessPaymentCommand(Guid OrderId) : IRequest<Result<bool>>;
}
