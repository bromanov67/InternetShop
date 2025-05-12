namespace InternetShop.Application.BusinessLogic.Cart
{
    public interface ICartRepository
    {
        Task<Guid> CreateCartAsync(Guid userId, CancellationToken cancellationToken);
        Task<Domain.Cart?> GetCartAsync(Guid cartId, CancellationToken cancellationToken);
        Task<Domain.Cart> AddProductToCartAsync(
            Guid cartId,
            string productId,
            int quantity,
            CancellationToken cancellationToken);
        Task DeleteCartAsync(Guid cartId, CancellationToken cancellationToken);
    }
}
