using FluentResults;
using MediatR;

namespace InternetShop.Application.BusinessLogic.User.Login
{
    public record LoginQuery(string Email, string Password) : IRequest<Result<string>>;
}
