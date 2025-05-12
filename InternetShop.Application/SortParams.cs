namespace InternetShop.Application
{
    public class SortParams
    {
        public string? OrderBy { get; set; }

        public SortDirectionEnum? SortDirection { get; set; }

        public enum SortDirectionEnum
        {
            Ascending,
            Descending
        }

    } 
}
