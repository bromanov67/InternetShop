using FluentResults;
using MediatR;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace InternetShop.Application.User.CreateUser
{
    public record CreateUserCommand : IRequest<Result<int>>
    {
        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }

        [JsonPropertyName("lastname")]
        public string Lastname { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
