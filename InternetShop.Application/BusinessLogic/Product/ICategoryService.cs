namespace InternetShop.Application.BusinessLogic.Product
{
    public interface ICategoryService
    {
        Task<Domain.Category> GetCategoryAsync(string categoryId, CancellationToken cancellationToken = default);
        Task<string> CreateCategoryAsync(Domain.Category category, CancellationToken cancellationToken = default);
        Task UpdateCategoryAsync(Domain.Category category, CancellationToken cancellationToken = default);
        Task DeleteCategoryAsync(string categoryId, CancellationToken cancellationToken = default);
        Task<PagedResult<Domain.Category>> GetCategoriesAsync(PageParams pagination,
            CancellationToken cancellationToken = default);
    }
}
