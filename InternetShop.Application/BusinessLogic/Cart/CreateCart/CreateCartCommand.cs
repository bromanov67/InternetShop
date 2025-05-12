using FluentResults;
using MediatR;
using MongoDB.Bson.Serialization.Attributes;

namespace InternetShop.Application.BusinessLogic.Cart.CreateCart
{

    public record CreateCartCommand(
        [property: BsonElement("userId")]
    Guid userId,

        [property: BsonElement("productId")]
    Guid productId
    ) : IRequest<Result<Guid>>;
}
