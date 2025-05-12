namespace InternetShop.Application.BusinessLogic.Product
{
    public interface ICatalogRepository
    {
        public Task<PagedResult<Domain.Product>> GetProductsAsync(ProductFilter filter, SortParams sort, PageParams pageParams, CancellationToken cancellationToken);

        public Task<Domain.Product> GetProductByIdAsync(string id, CancellationToken cancellationToken);

        public Task CreateProductAsync(Domain.Product product, CancellationToken cancellationToken);

        public Task UpdateProductAsync(Domain.Product product, CancellationToken cancellationToken);

        public Task DeleteProductByIdAsync(string id, CancellationToken cancellationToken);
    }
}
