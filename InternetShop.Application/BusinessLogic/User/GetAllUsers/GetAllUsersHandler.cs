using InternetShop.Application.BusinessLogic.User.DTO;
using InternetShop.Application.BusinessLogic.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InternetShop.Application.BusinessLogic.User.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDataDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(
            IIdentityService identityService,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<List<UserDataDto>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting users with filters: {@Filters}", new
                {
                    request.RoleFilters,
                    request.SearchTerm,
                    request.PageNumber,
                    request.PageSize
                });

                var users = await _identityService.GetAllUsersAsync(
                    request.RoleFilters,
                    request.SearchTerm,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting users");
                throw; // Исключение будет обработано на уровне контроллера
            }
        }
    }
}