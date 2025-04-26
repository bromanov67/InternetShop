using InternetShop.Application;
using InternetShop.Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace InternetShop.Database
{
    public static class ProductExtensions
    {
        public static FilterDefinition<Product> FilterProducts(this ProductFilter filter)
        {
            var filterDefinition = Builders<Product>.Filter.Empty;

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    filterDefinition &= Builders<Product>.Filter.Eq(p => p.Name, filter.Name);
                }

                if (!string.IsNullOrEmpty(filter.Categories))
                {
                    var categoryFilter = Builders<Product>.Filter.ElemMatch(
                        p => p.Categories,
                        Builders<string>.Filter.Eq("Category", filter.Categories)
                    );
                    filterDefinition &= categoryFilter;
                }

                if (filter.MinPrice.HasValue)
                {
                    filterDefinition &= Builders<Product>.Filter.Gte(p => p.Price, filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    filterDefinition &= Builders<Product>.Filter.Lte(p => p.Price, filter.MaxPrice.Value);
                }
            }

            return (filterDefinition);
        }

        public static SortDefinition<Product> Sort(this SortParams sortParams)
        {
            var sortDefinition = Builders<Product>.Sort.Ascending(p => p.Name);

            if (sortParams.SortDirection == SortParams.SortDirectionEnum.Ascending)
            {
                sortDefinition = Builders<Product>.Sort.Ascending(sortParams.OrderBy);
            }

            if (sortParams.SortDirection == SortParams.SortDirectionEnum.Descending)
            {
                sortDefinition = Builders<Product>.Sort.Descending(sortParams.OrderBy);
            }
            return sortDefinition;
        }


        public static async Task<PagedResult<T>> ToPagedAsync<T>(this IFindFluent<T, T> query, PageParams pageParams, CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountDocumentsAsync(cancellationToken);
            var page = pageParams.Page;
            var pageSize = pageParams.PageSize;

            var data = await query
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>(data, (int)totalCount);
        }
    }
}