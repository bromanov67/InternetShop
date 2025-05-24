using FluentResults;
using InternetShop.Application.BusinessLogic.User.Interfaces;
using InternetShop.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InternetShop.Application.BusinessLogic.User.UpdateUser
{
    // UpdateUserHandler.cs
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public UpdateUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            return await _identityService.UpdateUserAsync(
                request.UserId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.Role,
                request.Password,
                cancellationToken);
        }
    }
}