using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace InternetShop.Application.User.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<Domain.User> _passwordHasher;

        public LoginQueryHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher<Domain.User> passwordHasher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }  

        public async Task<Result<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            // 1. Найти пользователя по Email
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            // 2. Проверить пароль
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Result.Fail("Неверный пароль.");
            }
            // 3. Создать токен
            var token = _tokenService.GenerateJwtToken(user);
            return Result.Ok(token);
        }

    }
}
