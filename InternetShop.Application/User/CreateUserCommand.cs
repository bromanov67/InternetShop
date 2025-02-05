using FluentResults;
using MediatR;
using System.Text.Json.Serialization;

namespace InternetShop.Application.User
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
