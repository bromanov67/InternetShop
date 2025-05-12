using System.Security.Claims;

namespace InternetShop.Application.BusinessLogic.User
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        ClaimsPrincipal? User { get; }
    }
}
