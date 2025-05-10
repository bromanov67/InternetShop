using FluentResults;
using InternetShop.Domain;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Product.AddProduct
{
    public record AddProductCommand(string Name, List<string> Categories, Dictionary<string, string> categoryProperties, decimal Price) : IRequest<Result<string>> { }
}
