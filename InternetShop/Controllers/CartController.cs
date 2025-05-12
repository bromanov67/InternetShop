using InternetShop.Application.BusinessLogic.Cart;
using InternetShop.Application.BusinessLogic.Cart.GetCart;
using InternetShop.Application.BusinessLogic.Product;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Web.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IRedisCacheService _cacheService;
        private readonly IMediator _mediator;

        public CartController(ICartRepository cartRepository, IRedisCacheService cacheService, IMediator mediator)
        {
            _cartRepository = cartRepository;
            _cacheService = cacheService;
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCart(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCartQuery(id), cancellationToken);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var cart = await _cartRepository.CreateCartAsync(userId, cancellationToken);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("addProduct")]
        public async Task<IActionResult> AddProduct(
            Guid cartId,
            string productId,
            string productName,
            decimal price,
            int quantity,
            CancellationToken cancellationToken)
        {
            try
            {
                var cart = await _cartRepository.AddProductToCartAsync(
                    cartId,
                    productId,
                    quantity,
                    cancellationToken);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
