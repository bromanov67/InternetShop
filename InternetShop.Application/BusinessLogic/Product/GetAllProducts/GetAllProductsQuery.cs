using FluentResults;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Product.GetAllProducts
{
    public record GetAllProductsQuery(ProductFilter filter, SortParams sort, PageParams pageParams) : IRequest<PagedResult<Domain.Product>>
    {
    }
}
