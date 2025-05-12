using FluentResults;
using MediatR;

namespace InternetShop.Application.BusinessLogic.User
{
    public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDataDto>>;
}