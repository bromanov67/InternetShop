using MongoDB.Bson.Serialization.Attributes;

namespace InternetShop.Domain
{
    public partial class Cart
    {
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
    }
}