using FluentResults;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Product.AddProduct
{
    public class AddProductHandler : IRequestHandler<AddProductCommand, Result<string>>
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IRedisCacheService _redisCategoryService;

        public AddProductHandler(ICatalogRepository catalogRepository, IRedisCacheService redisCategoryService)
        {
            _catalogRepository = catalogRepository;
            _redisCategoryService = redisCategoryService;
        }

        public async Task<Result<string>> Handle(AddProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                //var categoryExists = await _redisCategoryService.CategoryExistsAsync(
                //    command.CategoryType,
                //    command.CategoryKey
                //);

                //if (!categoryExists)
                //{
                //    return Result.Fail<string>("Категория не существует");
                //}

                var product = new Domain.Product(command.Name, command.Categories, command.categoryProperties, command.Price);
                await _catalogRepository.CreateProductAsync(product, cancellationToken);
                return Result.Ok(product.Id);
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Ошибка: {ex.Message}");
            }
        }
    }
}
