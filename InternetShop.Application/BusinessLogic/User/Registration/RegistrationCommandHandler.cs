using InternetShop.Application.BusinessLogic.User.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InternetShop.Application.BusinessLogic.User.Registration
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, IActionResult>
    {
        private readonly ILogger<RegistrationCommandHandler> _logger;
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public RegistrationCommandHandler(ILogger<RegistrationCommandHandler> logger,
            IIdentityService identityService, 
            ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _identityService = identityService;
        }

        public async Task<IActionResult> Handle(RegistrationCommand command, CancellationToken ct)
        {
            try
            {
                // Создаем объект с данными (но не сохраняем как отдельную сущность)
                var userData = new Domain.User
                {
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email,
                    Role = command.Role
                };

                var result = await _identityService.RegisterAsync(
                    command.Email,
                    command.Password,
                    userData,
                    ct);

                if (result.IsFailed)
                    return new BadRequestObjectResult(result.Errors);

                var token = _tokenService.GenerateJwtToken(userData, ct);
                return new OkObjectResult(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return new StatusCodeResult(500);
            }
        }
    }
}