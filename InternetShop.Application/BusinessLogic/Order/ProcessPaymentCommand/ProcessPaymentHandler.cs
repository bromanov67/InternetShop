namespace InternetShop.Application.BusinessLogic.Order.ProcessPaymentCommand
{
    using FluentResults;
    using InternetShop.Application.BusinessLogic.Order.DTO;
    using InternetShop.Application.BusinessLogic.Order.Interfaces;
    using MediatR;
    using System.Threading;
    using Xunit.Sdk;

    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, Result<bool>>
    {
        private readonly IOrderRepository _orderRepository;

        public ProcessPaymentHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<bool>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);

                if (order == null)
                    return Result.Fail<bool>("Order not found");

                if (order.Status != OrderStatus.Created)
                    return Result.Fail<bool>("Order is already processed or invalid state");
                var success = await _orderRepository.UpdateOrderStatusAsync(
                    request.OrderId,
                    OrderStatus.Paid,
                    cancellationToken);

                if (!success)
                    return Result.Fail<bool>("Failed to update order status");

                var updatedOrder = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
                return updatedOrder == null
                    ? Result.Fail<bool>("Order not found after update")
                    : Result.Ok(true);
            }
            catch (Exception)
            { 
                return Result.Fail<bool>("Internal server error");
            }
        }
    }
}
