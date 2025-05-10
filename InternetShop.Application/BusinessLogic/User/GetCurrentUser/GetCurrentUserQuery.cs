using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Application.BusinessLogic.User.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<IActionResult>
    {
        public string UserId { get; init; }
    }
}