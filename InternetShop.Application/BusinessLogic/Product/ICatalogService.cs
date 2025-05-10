namespace InternetShop.Application.BusinessLogic.Product
{
    public interface ICatalogService
    {
        Task UpdateProductAsync(Domain.Product product, CancellationToken cancellationToken);
    }
}