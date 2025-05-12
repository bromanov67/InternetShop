using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace InternetShop.Application.BusinessLogic.User.GetCurrentUser
{
    public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, IActionResult>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetCurrentUserHandler> _logger;

        public GetCurrentUserHandler(
            IIdentityService identityService,
            ILogger<GetCurrentUserHandler> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }


        public async Task<IActionResult> Handle(GetCurrentUserQuery command, CancellationToken cancellationToken)
        {
            // Проверка: не пустой ли UserId
            if (string.IsNullOrWhiteSpace(command.UserId))
            {
                _logger.LogWarning("UserId is null or empty in GetCurrentUserQuery");
                return new UnauthorizedObjectResult(new { Error = "User identifier is missing" });
            }

            // Проверка валидности Guid
            if (!Guid.TryParse(command.UserId, out var userId))
            {
                _logger.LogWarning("Invalid user ID format: {UserId}", command.UserId);
                return new UnauthorizedObjectResult(new { Error = "Invalid user identifier" });
            }

            try
            {
                // Получаем данные пользователя
                var result = await _identityService.GetCurrentUserDataAsync(userId, cancellationToken);

                if (result.IsFailed)
                {
                    _logger.LogWarning("Failed to get current user data for ID {UserId}: {Error}", userId, result.Errors);
                    return new NotFoundObjectResult(new { Error = result.Errors });
                }

                return new OkObjectResult(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user data for ID {UserId}", userId);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}