using InternetShop.Application.BusinessLogic.Order.AddOrderItem;
using InternetShop.Application.BusinessLogic.Order.CreateOrder;
using InternetShop.Application.BusinessLogic.Order.DTO;
using InternetShop.Application.BusinessLogic.Order.GetAllOrders;
using InternetShop.Application.BusinessLogic.Order.GetUserOrders;
using InternetShop.Application.BusinessLogic.Order.ProcessPaymentCommand;
using InternetShop.Application.BusinessLogic.Order.UpdateOrderStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Web.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderCommand request)
        {
            var command = new CreateOrderCommand(
                request.UserId,
                request.PaymentTypeId,
                request.Items
            );

            var result = await _mediator.Send(command);

            return result.IsFailed
                ? BadRequest(result.Errors)
                : Ok(result.Value);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var query = new GetAllOrdersQuery();
            var result = await _mediator.Send(query);

            return result.IsFailed
                ? BadRequest(result.Errors)
                : Ok(result.Value);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(Guid userId)
        {
            var query = new GetUserOrdersQuery(userId);
            var result = await _mediator.Send(query);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
        Guid orderId,
        [FromBody] UpdateOrderStatusCommand request)
        {
            var command = new UpdateOrderStatusCommand(orderId, request.NewStatus);
            var result = await _mediator.Send(command);
            return result.IsFailed ? BadRequest(result.Errors) : Ok(result.Value);
        }

        [HttpPost("{orderId}/items")]
        public async Task<IActionResult> AddOrderItem(
            Guid orderId,
            [FromBody] AddOrderItemCommand command)
        {
            var item = new AddOrderItemDto(command.Item.ProductId, command.Item.Quantity);
            var result = await _mediator.Send(command);
            return result.IsFailed ? BadRequest(result.Errors) : Ok(result.Value);
        }

        [HttpPost("{orderId}/payment")]
        public async Task<IActionResult> ProcessPayment(Guid orderId, CancellationToken ct)
        {
            var result = await _mediator.Send(new ProcessPaymentCommand(orderId), ct);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }
    }
}
