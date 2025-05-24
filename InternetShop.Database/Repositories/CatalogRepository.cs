using InternetShop.Application;
using InternetShop.Application.BusinessLogic.Product;
using InternetShop.Database.Services;
using InternetShop.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InternetShop.Database.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly IMongoCollection<Product> _productCollection;

        public CatalogRepository(IMongoDatabase database)
        {
            _productCollection = database.GetCollection<Product>("products");
        }

        public async Task<PagedResult<Product>> GetProductsAsync(ProductFilter filter, SortParams sort,
            PageParams pageParams, CancellationToken cancellationToken)
        {
            var filterDefinition = Builders<Product>.Filter.Empty;

            if (!string.IsNullOrEmpty(filter.Name))
            {
                filterDefinition &= Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(filter.Name, "i"));
            }

            if (!string.IsNullOrEmpty(filter.CategoryProperties))
            {
                filterDefinition &= Builders<Product>.Filter.AnyEq(p => p.Categories, filter.CategoryProperties);
            }

            if (filter.MinPrice.HasValue)
            {
                filterDefinition &= Builders<Product>.Filter.Gte(p => p.Price, filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                filterDefinition &= Builders<Product>.Filter.Lte(p => p.Price, filter.MaxPrice.Value);
            }

            var sortDefinition = sort.SortDirection == SortParams.SortDirectionEnum.Ascending
                ? Builders<Product>.Sort.Ascending(sort.OrderBy)
                : Builders<Product>.Sort.Descending(sort.OrderBy);

            var query = _productCollection.Find(filterDefinition).Sort(sortDefinition);

            var totalCount = await query.CountDocumentsAsync(cancellationToken);
            var data = await query
                .Skip((pageParams.Page - 1) * pageParams.PageSize)
                .Limit(pageParams.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>(data, (int)totalCount, pageParams.Page, pageParams.PageSize);
        }

        public async Task<Product> GetProductByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _productCollection
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CreateProductAsync(Product product, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = ObjectId.GenerateNewId().ToString();
            }
            await _productCollection.InsertOneAsync(product, cancellationToken: cancellationToken);
        }

        public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            await _productCollection.ReplaceOneAsync(
                p => p.Id == product.Id,
                product,
                new ReplaceOptions { IsUpsert = false },
                cancellationToken);
        }

        public async Task DeleteProductByIdAsync(string id, CancellationToken cancellationToken)
        {
            await _productCollection.DeleteOneAsync(
                p => p.Id == id,
                cancellationToken);
        }

        public async Task<Dictionary<string, decimal>> GetProductPricesAsync(
            IEnumerable<string> productIds,
            CancellationToken cancellationToken)
        {
            var filter = Builders<Product>.Filter.In(p => p.Id, productIds);

            var products = await _productCollection
                .Find(filter)
                .Project(p => new { p.Id, p.Price })
                .ToListAsync(cancellationToken);

            return products.ToDictionary(p => p.Id, p => p.Price);
        }


        public async Task<Dictionary<string, string>> GetProductNamesAsync(
        IEnumerable<string> productIds,
        CancellationToken cancellationToken)
        {
            var filter = Builders<Product>.Filter.In(p => p.Id, productIds.Select(x => x.ToString()));

            var products = await _productCollection
                .Find(filter)
                .Project(p => new { Id = p.Id, p.Name })
                .ToListAsync(cancellationToken);

            return products.ToDictionary(
                p => p.Id,
                p => p.Name
            );
        }

        public class ProductDocument
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = null!;
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public bool IsActive { get; set; }
            public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        }
    }
}