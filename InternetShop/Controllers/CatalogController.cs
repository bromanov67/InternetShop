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
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] ProductFilter filter = null,
            [FromQuery] SortParams sort = null,
            [FromQuery] PageParams pageParams = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Initialize defaults with proper null checks
                filter ??= new ProductFilter();
                sort ??= new SortParams
                {
                    OrderBy = "Name",
                    SortDirection = SortParams.SortDirectionEnum.Ascending
                };
                pageParams ??= new PageParams
                {
                    Page = 1,
                    PageSize = 10
                };

                // Validate parameters
                if (string.IsNullOrWhiteSpace(sort.OrderBy))
                {
                    sort.OrderBy = "Name";
                }

                if (pageParams.Page < 1) pageParams.Page = 1;
                if (pageParams.PageSize < 1 || pageParams.PageSize > 100) pageParams.PageSize = 10;

                var query = new GetAllProductsQuery(filter, sort, pageParams);
                var result = await _mediator.Send(query, cancellationToken);

                return Ok(new
                {
                    result.Data,
                    result.TotalCount,
                    pageParams.Page,
                    pageParams.PageSize
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
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
