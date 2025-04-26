using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace InternetShop.Domain
{
    public class Cart
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("ProductId")]
        public Guid ProductId { get; set; }

        [BsonElement("UserId")]
        public Guid UserId { get; set; }

        [BsonElement("Items")]
        public List<CartItem> Items { get; set; } = new();

        public class CartItem
        {
            [BsonElement("ProductId")]
            public string ProductId { get; set; } = null!;

            [BsonElement("Quantity")]
            public int Quantity { get; set; }

            [BsonElement("ProductName")]
            public string ProductName { get; set; } = null!;

            [BsonElement("Price")]
            public decimal Price { get; set; }
        }

        // Parameterless constructor for MongoDB
        public Cart() { }

        // Primary constructor
        [BsonConstructor]
        public Cart(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
        }

        // Secondary constructor
        public Cart(Guid userId, Guid productId) : this(userId)
        {
            ProductId = productId;
            Items.Add(new CartItem
            {
                ProductId = productId.ToString(),
                Quantity = 1
            });
        }
    }
}