using FluentResults;
using InternetShop.Domain;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InternetShop.Application.User.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<int>>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<int>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = new Domain.User(command.Firstname, command.Lastname, command.Email);
            await _userRepository.CreateAsync(user, cancellationToken);
            return Result.Ok(user.Id);
        }
    }
}
