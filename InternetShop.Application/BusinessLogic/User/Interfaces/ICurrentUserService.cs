using System.Security.Claims;

namespace InternetShop.Application.BusinessLogic.User.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        ClaimsPrincipal? User { get; }
    }
}
