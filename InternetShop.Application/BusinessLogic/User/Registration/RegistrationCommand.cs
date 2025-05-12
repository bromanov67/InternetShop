using InternetShop.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace InternetShop.Application.BusinessLogic.User.Registration
{
    public class RegistrationCommand : IRequest<IActionResult>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoleEnum Role { get; set; } = RoleEnum.Client;
    }
}
