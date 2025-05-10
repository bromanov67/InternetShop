namespace InternetShop.Domain
{
    public class Favorites
    {
        public Guid Id { get; set; }

        public Guid userId { get; set; }

        public string ProductId { get; set; } = null!;
    }
}
