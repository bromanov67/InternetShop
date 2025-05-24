using InternetShop.Application.BusinessLogic.User.DTO;
using InternetShop.Application.BusinessLogic.User.GetAllUsers;
using InternetShop.Application.BusinessLogic.User.Interfaces;
using InternetShop.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace InternetShop.Application.BusinessLogic.User.GetClients
{
    public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, IEnumerable<UserDataDto>>
    {
        private readonly IIdentityService _identityService;

        public GetClientsQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<IEnumerable<UserDataDto>> Handle(
            GetClientsQuery request,
            CancellationToken cancellationToken)
        {
            // Получаем только клиентов через IdentityService
            var clientEntities = await _identityService.GetUsersInRoleAsync(RoleEnum.Client, cancellationToken);

            // Применяем пагинацию
            var paginatedClients = clientEntities
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);

            return paginatedClients;
        }
    }
}
