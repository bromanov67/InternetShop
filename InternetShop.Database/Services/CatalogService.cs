using InternetShop.Application;
using InternetShop.Application.BusinessLogic.Product;
using InternetShop.Domain;
using MongoDB.Bson;

namespace InternetShop.Database.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _repository;
        private readonly IRedisCacheService _cache;
        private readonly ICategoryService _categoryService;

        public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            // Бизнес-логика
            if (product.Price < 0)
                throw new Exception("Price cannot be negative");

            /*      // Валидация зависимостей
                  if (!await _categoryService.ExistsAsync(product.Categor, cancellationToken))
                      throw new Exception("Invalid category");*/

            // Работа с репозиторием
            await _repository.UpdateProductAsync(product, cancellationToken);

            // Управление кэшем
            await _cache.RemoveAsync($"product:{product.Id}", cancellationToken);
        }
    }
}