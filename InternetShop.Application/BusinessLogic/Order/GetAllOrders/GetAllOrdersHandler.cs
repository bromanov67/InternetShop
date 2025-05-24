using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using InternetShop.Application.BusinessLogic.Order.Interfaces;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Order.GetAllOrders
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, Result<List<OrderDto>>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetAllOrdersHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<List<OrderDto>>> Handle(
            GetAllOrdersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync(cancellationToken);
                var orderDtos = orders.Select(MapToDto).ToList();

                return Result.Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return Result.Fail<List<OrderDto>>(ex.Message);
            }
        }

        private OrderDto MapToDto(Order order) => new(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.Status,
            order.TotalAmount,
                order.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.Quantity)).ToList());
    }
}
