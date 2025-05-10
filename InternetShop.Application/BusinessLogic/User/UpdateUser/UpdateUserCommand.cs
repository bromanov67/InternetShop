using FluentResults;
using InternetShop.Domain;
using MediatR;

namespace InternetShop.Application.BusinessLogic.User.UpdateUser
{
    // UpdateUserCommand.cs
    public record UpdateUserCommand(
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        RoleEnum Role,
        string? Password = null) : IRequest<Result>;
}