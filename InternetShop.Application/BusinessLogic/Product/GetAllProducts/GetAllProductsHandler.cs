using MediatR;

namespace InternetShop.Application.BusinessLogic.Product.GetAllProducts
{
    // Обработчик запроса для получения всех продуктов
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<Domain.Product>>
    {
        private readonly ICatalogRepository _repository;
        private readonly IRedisCacheService _cache;

        public GetAllProductsQueryHandler(
            ICatalogRepository repository,
            IRedisCacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResult<Domain.Product>> Handle(
            GetAllProductsQuery request,
            CancellationToken ct)
        {
            // Формирование ключа кэша на основе параметров запроса
            var cacheKey = $"products:{request.filter}:{request.sort}:{request.pageParams}";

            // Проверка наличия данных в кэше
            var cached = await _cache.GetAsync<PagedResult<Domain.Product>>(cacheKey, ct);
            if (cached != null) return cached;

            // Получение данных из репозитория, если они не найдены в кэше
            var products = await _repository.GetProductsAsync(request.filter, request.sort, request.pageParams, ct);

            // Сохранение результатов в кэше
            if (products != null)
            {
                await _cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(10), ct);
            }

            return products;
        }

    }
}