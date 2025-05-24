using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Order.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<Result<List<OrderDto>>>
    {
    }
}
