// DeleteUserCommand.cs
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InternetShop.Application.BusinessLogic.User.DeleteUser
{
    public record DeleteUserCommand(Guid UserId) : IRequest<Result>;
}