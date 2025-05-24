using FluentResults;
using InternetShop.Application.BusinessLogic.User.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Application.BusinessLogic.User.DeleteUser
{
    // DeleteUserHandler.cs
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public DeleteUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.DeleteUserAsync(request.UserId, cancellationToken);
        }
    }
}