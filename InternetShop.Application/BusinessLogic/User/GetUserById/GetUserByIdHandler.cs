using FluentResults;
using InternetShop.Application.BusinessLogic.User.GetAllUsers;
using InternetShop.Application.BusinessLogic.User.Registration;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Application.BusinessLogic.User
{
    // GetUserByIdQueryHandler.cs
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDataDto>>
    {
        private readonly IIdentityService _identityService;

        public GetUserByIdQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<UserDataDto>> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _identityService.GetUserByIdAsync(request.Id, cancellationToken);
        }
    }
}