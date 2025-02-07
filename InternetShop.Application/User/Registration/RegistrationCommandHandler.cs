using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Application.User.Registration
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, IActionResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<Domain.User> _passwordHasher;

        public RegistrationCommandHandler(IUserRepository userRepository, IPasswordHasher<Domain.User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<IActionResult> Handle(RegistrationCommand command, CancellationToken cancellationToken)
        {
            // Проверка на существующего пользователя
            var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (existingUser != null)
            {
                return new BadRequestObjectResult("Пользователь с таким email уже существует.");
            }
           
            var user = new Domain.User
            {
                Firstname = command.Fistname,
                Lastname = command.Lastname,
                Email = command.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, command.Password);

            await _userRepository.AddAsync(user); // Передаем сущность

            return new OkResult();
        }
    }
}
