using InternetShop.Application.BusinessLogic.User;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InternetShop.Database.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;
    }
}
