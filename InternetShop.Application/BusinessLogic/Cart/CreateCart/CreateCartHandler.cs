using FluentResults;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Cart.CreateCart
{
    public class CreateCartHandler : IRequestHandler<CreateCartCommand, Result<Guid>>
    {
        private readonly ICartRepository _cartRepository;

        public CreateCartHandler(ICartRepository cartService)
        {
            _cartRepository = cartService;
        }
        public async Task<Result<Guid>> Handle(CreateCartCommand command, CancellationToken cancellationToken)
        {
            var cart = new Domain.Cart(command.userId, command.productId);
            await _cartRepository.CreateCartAsync(cart.Id, cancellationToken);
            return Result.Ok(cart.Id);
        }
    }
}
