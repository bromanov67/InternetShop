using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.BusinessLogic.Order.GetUserOrders
{
    public record GetUserOrdersQuery(Guid UserId) : IRequest<Result<List<OrderDto>>>;
}
