using InternetShop.Application.BusinessLogic.Cart;
using InternetShop.Domain;
using MongoDB.Driver;
using static InternetShop.Domain.Cart;

namespace InternetShop.Database
{
    public class CartRepository : ICartRepository
    {
        private readonly IMongoCollection<Cart> _collection;

        public CartRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Cart>("carts");
        }

        public async Task<Cart?> GetCartAsync(Guid cartId, CancellationToken cancellationToken)
        {
            return await _collection
                .Find(c => c.Id == cartId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Guid> CreateCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            var cart = new Cart(userId);

            await _collection.InsertOneAsync(cart, cancellationToken: cancellationToken);
            return cart.Id;
        }

        public async Task<Cart> AddProductToCartAsync(
            Guid cartId,
            string productId,
            int quantity,
            CancellationToken cancellationToken)
        {
            var item = new Cart.CartItem
            {
                ProductId = productId,
                Quantity = quantity
            };

            var filter = Builders<Cart>.Filter.Eq(c => c.Id, cartId);
            var update = Builders<Cart>.Update.Push(c => c.Items, item);

            var options = new FindOneAndUpdateOptions<Cart>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _collection.FindOneAndUpdateAsync(
                filter,
                update,
                options,
                cancellationToken);
        }

        public async Task DeleteCartAsync(Guid cartId, CancellationToken cancellationToken)
        {
            await _collection.DeleteOneAsync(
                c => c.Id == cartId,
                cancellationToken);
        }
    }
}
