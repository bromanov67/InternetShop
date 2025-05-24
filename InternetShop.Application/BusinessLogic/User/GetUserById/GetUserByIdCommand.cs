using FluentResults;
using InternetShop.Application.BusinessLogic.User.DTO;
using MediatR;

namespace InternetShop.Application.BusinessLogic.User
{
    public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDataDto>>;
}