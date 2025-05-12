using InternetShop.Application;
using InternetShop.Application.BusinessLogic.Product;
using InternetShop.Domain;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

public class CategoryService : ICategoryService
{
    private readonly IMongoCollection<Category> _collection;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        IMongoDatabase database,
        IRedisCacheService cache,
        ILogger<CategoryService> logger)
    {
        _collection = database.GetCollection<Category>("categories");
        _cache = cache;
        _logger = logger;
    }

/*    public async Task<bool> ExistsAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(categoryId))
            return false;

        // Сначала проверяем кэш
        var cacheKey = $"category:exists:{categoryId}";
        var cached = await _cache.GetAsync<bool?>(cacheKey, cancellationToken);
        if (cached.HasValue) return cached.Value;

        // Если нет в кэше - проверяем в БД
        var exists = await _collection.Find(c => c.Id == categoryId)
            .AnyAsync(cancellationToken);

        // Кэшируем результат на 10 минут
        await _cache.SetAsync(cacheKey, exists, TimeSpan.FromMinutes(10), cancellationToken);
        return exists;
    }*/

    public async Task<Category> GetCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"category:{categoryId}";

        // Пытаемся получить из кэша
        var cachedCategory = await _cache.GetAsync<Category>(cacheKey, cancellationToken);
        if (cachedCategory != null) return cachedCategory;

        // Запрос к MongoDB
        var category = await _collection.Find(c => c.Id == categoryId)
            .FirstOrDefaultAsync(cancellationToken);

        if (category != null)
        {
            // Кэшируем на 2 часа
            await _cache.SetAsync(cacheKey, category, TimeSpan.FromHours(2), cancellationToken);
        }

        return category;
    }

    public async Task<string> CreateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(category.Name))
            throw new ArgumentException("Category name cannot be empty");

        // Генерация ID если не указан
        if (string.IsNullOrEmpty(category.Id))
            category.Id = ObjectId.GenerateNewId().ToString();

        // Сохранение в MongoDB
        await _collection.InsertOneAsync(category, cancellationToken: cancellationToken);

        // Инвалидация кэша
        await _cache.RemoveAsync(category.Id, cancellationToken);

        return category.Id;
    }

    public async Task UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        // Валидация
        if (string.IsNullOrEmpty(category.Id))
            throw new ArgumentException("Category ID is required");

        // Обновление в MongoDB
        var result = await _collection.ReplaceOneAsync(
            c => c.Id == category.Id,
            category,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);

        if (result.MatchedCount == 0)
            throw new Exception("Category not found");

        // Инвалидация кэша
        await _cache.RemoveAsync($"category:{category.Id}", cancellationToken);
    }

    public async Task DeleteCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        // Удаление из MongoDB
        var result = await _collection.DeleteOneAsync(c => c.Id == categoryId, cancellationToken);

        if (result.DeletedCount == 0)
            throw new Exception("Category not found");

        // Инвалидация кэша
        await _cache.RemoveAsync($"category:{categoryId}", cancellationToken);
        await _cache.RemoveAsync($"category:exists:{categoryId}", cancellationToken);
    }

    public async Task<PagedResult<Category>> GetCategoriesAsync(
        PageParams pagination,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"categories:list:{pagination.GetHashCode()}";

        // Пытаемся получить из кэша
        var cached = await _cache.GetAsync<PagedResult<Category>>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        // Построение запроса
        var query = _collection.Find(_ => true)
            .Sort(Builders<Category>.Sort.Ascending(c => c.Name));

        // Пагинация
        var total = await query.CountDocumentsAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<Category>(items, total);

        // Кэшируем на 5 минут
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return result;
    }
}
