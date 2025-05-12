namespace InternetShop.Application
{
    public class ProductFilter
    {
        public string? Name { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? Categories { get; set; }

        public string? CategoryProperties { get; set; }
    }
}
