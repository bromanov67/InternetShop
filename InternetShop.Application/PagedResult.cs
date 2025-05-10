namespace InternetShop.Application
{
    public class PagedResult<T>
    {
        public PagedResult(List<T> data, long totalCount)
        {
            Data = data;
            TotalCount = totalCount;
        }

        public List<T> Data { get; set; }
        public long TotalCount { get; set; }
    }
}
