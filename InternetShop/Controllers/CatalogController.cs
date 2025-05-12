using InternetShop.Application;
using InternetShop.Application.BusinessLogic.Product;
using InternetShop.Application.BusinessLogic.Product.AddProduct;
using InternetShop.Application.BusinessLogic.Product.GetAllProducts;
using InternetShop.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace InternetShop.Web.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IMediator _mediator;
        private readonly IConnectionMultiplexer _redis;
        public CatalogController(ICatalogRepository catalogRepository, IMediator mediator, IConnectionMultiplexer redis)
        {
            _catalogRepository = catalogRepository;
            _mediator = mediator;
            _redis = redis;
        }

        [HttpGet("productById")]
        public async Task<IActionResult> GetProductById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _catalogRepository.GetProductByIdAsync(id, cancellationToken);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductFilter filter,
                                                        [FromQuery] SortParams sort,
                                                        [FromQuery] PageParams pageParams,
                                                        CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllProductsQuery(filter, sort, pageParams);
                var products = await _mediator.Send(query, cancellationToken);

                return Ok(products.Data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return CreatedAtAction(nameof(GetProductById), new { id = result.Value }, new { Id = result.Value });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(Product product, CancellationToken cancellationToken)
        {
            try
            {
                await _catalogRepository.UpdateProductAsync(product, cancellationToken);
                return Ok(product);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _catalogRepository.DeleteProductByIdAsync(id, cancellationToken);
                return Ok(id);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }
    }
}
