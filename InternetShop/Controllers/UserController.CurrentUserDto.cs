using InternetShop.Domain;

namespace InternetShop.Controllers
{
    public partial class UserController
    {
        // DTO для ответа
        public record CurrentUserDto
        {
            public Guid Id { get; init; }
            public string Email { get; init; }
            public string Firstname { get; init; }
            public string Lastname { get; init; }
            public RoleEnum Role { get; init; }
        }
    }
}