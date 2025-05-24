/*using FluentResults;
using InternetShop.Application.BusinessLogic.Product;
using InternetShop.Application.BusinessLogic.User.Interfaces;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Cart.AddToCart
{
    public class AddToCartHandler : IRequestHandler<AddToCartCommand, Result>
    {
        private readonly ICatalogRepository _products;
        private readonly IIdentityService _users;

        public async Task<Result> Handle(AddToCartCommand command, CancellationToken ct)
        {
            // 1. Получаем продукт из MongoDB
            var product = await _products.GetProductByIdAsync(command.ProductId.ToString(), ct);
            if (product == null) return Result.Fail("Product not found");

            // 2. Получаем пользователя с корзиной из PostgreSQL
            var user = await _users.GetUserByIdAsync(command.UserId, ct);
            if (user == null) return Result.Fail("User not found");

            // 3. Добавляем в корзину
            user.Cart.AddItem(product.Id, product.Name, product.Price, command.Quantity);

            // 4. Сохраняем
            await _users.up(user);
            return Result.Ok();
        }
    }
}
*/