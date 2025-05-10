using InternetShop.Domain;

namespace InternetShop.Application.BusinessLogic
{
    public record UserDataDto(
        Guid Id,
        string Email,
        string FirstName,
        string LastName,
        RoleEnum Role);
}