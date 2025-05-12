using InternetShop.Domain;
using System.Text.Json.Serialization;

namespace InternetShop.Application.BusinessLogic.User.UpdateUser
{
    // DTO для обновления пользователя
    public class UserUpdateDto
    {
        public string? Firstname { get; init; }
        public string? Lastname { get; init; }
        public string? Email { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public RoleEnum? Role { get; init; }
    }
}