using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InternetShop.Domain
{
    public class Product
    {
        public Product(string name, List<string> categories, Dictionary<string, string> categoryProperties, decimal price)
        {
            Name = name;
            Categories = categories;
            CategoryProperties = categoryProperties;
            Price = price;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public List<string> Categories { get; set; }

        public Dictionary<string, string> CategoryProperties { get; set; }
    }
}
