using InternetShop.Domain;

namespace InternetShop.Application.BusinessLogic.User
{
    // DTO для ответа
    public record CurrentUserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Role { get; init; }
    }
}