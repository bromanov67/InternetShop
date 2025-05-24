using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using InternetShop.Application.BusinessLogic.Order.Interfaces;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Order.GetUserOrders
{
    public class GetUserOrdersHandler : IRequestHandler<GetUserOrdersQuery, Result<List<OrderDto>>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetUserOrdersHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<List<OrderDto>>> Handle(
            GetUserOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetOrderByUserIdAsync(request.UserId, cancellationToken);

            var orderDtos = orders.Select(o => new OrderDto(
                o.Id,
                o.UserId,
                o.CreatedAt,
                o.Status,
                o.TotalAmount,
                o.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity)).ToList()
            )).ToList();

            return Result.Ok(orderDtos);
        }
    }
}
