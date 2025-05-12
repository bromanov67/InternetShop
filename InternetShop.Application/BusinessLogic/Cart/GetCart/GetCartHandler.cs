using FluentResults;
using InternetShop.Application.BusinessLogic.Product;
using MediatR;
using MongoDB.Driver;

namespace InternetShop.Application.BusinessLogic.Cart.GetCart
{
    public class GetCartHandler : IRequestHandler<GetCartQuery, Result<Domain.Cart>>
    {
        private readonly IMongoCollection<Domain.Cart> _cartsCollection;
        private readonly IRedisCacheService _cacheService;

        public GetCartHandler(
            IMongoDatabase database,
            IRedisCacheService cacheService)
        {
            _cartsCollection = database.GetCollection<Domain.Cart>("carts");
            _cacheService = cacheService;
        }

        public async Task<Result<Domain.Cart>> Handle(
            GetCartQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Проверка кэша Redis
                var cacheKey = $"cart:{request.CartId}";
                var cachedCart = await _cacheService.GetAsync<Domain.Cart>(cacheKey, cancellationToken);
                if (cachedCart != null)
                {
                    return Result.Ok(cachedCart);
                }

                // Запрос к MongoDB
                var cart = await _cartsCollection
                    .Find(c => c.Id == request.CartId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (cart == null)
                {
                    return Result.Fail<Domain.Cart>("Cart not found");
                }

                // Кэширование на 5 минут
                await _cacheService.SetAsync(
                    cacheKey,
                    cart,
                    TimeSpan.FromMinutes(5),
                    cancellationToken);

                return Result.Ok(cart);
            }
            catch (Exception ex)
            {
                return Result.Fail<Domain.Cart>($"Failed to get cart: {ex.Message}");
            }
        }
    }
}