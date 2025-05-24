namespace InternetShop.Application
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; }
        public long TotalCount { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PagedResult(IEnumerable<T> data, long totalCount, int page, int pageSize)
        {
            Data = data ?? Enumerable.Empty<T>();
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }
}
