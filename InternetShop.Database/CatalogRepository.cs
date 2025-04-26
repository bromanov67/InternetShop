using InternetShop.Application;
using InternetShop.Application.BusinessLogic.Product;
using InternetShop.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InternetShop.Database
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
            var query = _productCollection.Find(filter.FilterProducts())
                .Sort(sort.Sort());

            var totalCount = await query.CountDocumentsAsync(cancellationToken);
            var data = await query
                .Skip((pageParams.Page - 1) * pageParams.PageSize)
                .Limit(pageParams.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>(data, totalCount);
        }

        public async Task<Product> GetProductByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _productCollection.Find(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CreateProductAsync(Product product, CancellationToken cancellationToken)
        {
            product.Id = ObjectId.GenerateNewId().ToString();
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
            await _productCollection.DeleteOneAsync(p => p.Id == id, cancellationToken);
        }


    }
}
