using FluentResults;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Cart.GetCart
{
    public record GetCartQuery(Guid CartId) : IRequest<Result<Domain.Cart>>;
}
