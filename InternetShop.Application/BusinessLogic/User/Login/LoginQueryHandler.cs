using FluentResults;
using InternetShop.Domain;
using MediatR;
using Microsoft.Extensions.Logging;


namespace InternetShop.Application.BusinessLogic.User.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<string>>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginQueryHandler> _logger;

        public LoginQueryHandler(
            IIdentityService identityService,
            ITokenService tokenService,
            ILogger<LoginQueryHandler> logger)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Проверка учетных данных
                var userIdResult = await _identityService.ValidateCredentialsAsync(
                    request.Email,
                    request.Password,
                    cancellationToken);

                if (userIdResult.IsFailed)
                {
                    _logger.LogWarning("Authentication failed for {Email}", request.Email);
                    return Result.Fail<string>(userIdResult.Errors);
                }

                // 2. Получение данных для токена
                var userEmail = await _identityService.GetUserEmailAsync(userIdResult.Value, cancellationToken);
                var userNames = await _identityService.GetUserNamesAsync(userIdResult.Value, cancellationToken);
                var userRole = await _identityService.GetUserRoleAsync(userIdResult.Value, cancellationToken);


                var userData = new Domain.User
                {
                    Id = userIdResult.Value,
                    Email = userEmail.Value,
                    FirstName = userNames.Value.FirstName,
                    LastName = userNames.Value.LastName,
                    Role = Enum.Parse<RoleEnum>(userRole.Value)
                };

                var token = _tokenService.GenerateJwtToken(userData, cancellationToken);
                _logger.LogInformation("Successful login for {Email}", request.Email);
                return Result.Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for {Email}", request.Email);
                return Result.Fail<string>("Login failed").WithError(ex.Message);
            }
        }
    }
}