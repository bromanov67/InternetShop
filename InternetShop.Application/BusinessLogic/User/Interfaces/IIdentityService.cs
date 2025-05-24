using FluentResults;
using InternetShop.Application.BusinessLogic.User.DTO;
using InternetShop.Application.BusinessLogic.User.GetAllUsers;
using InternetShop.Domain;

namespace InternetShop.Application.BusinessLogic.User.Interfaces
{
    public partial interface IIdentityService
    {
        Task<Result<string>> LoginAsync(string email, string password, CancellationToken cancellationToken);

        Task<Result<Guid>> RegisterAsync(string email, string password, Domain.User domainUser, CancellationToken cancellationToken);

        Task<Result<Guid>> ValidateCredentialsAsync(
            string email,
            string password,
            CancellationToken ct);

        Task<Result<string>> GetUserEmailAsync(
            Guid userId,
            CancellationToken ct);

        Task<Result<(string FirstName, string LastName)>> GetUserNamesAsync(
            Guid userId,
            CancellationToken ct);

        Task<Result<string>> GetUserRoleAsync(
            Guid userId,
            CancellationToken ct);

        Task<Result<CurrentUserDto>> GetCurrentUserDataAsync(Guid userId, CancellationToken ct);

        Task<List<UserDataDto>> GetAllUsersAsync(
           List<RoleEnum>? roleFilters,
           string? searchTerm,
           int pageNumber,
           int pageSize,
           CancellationToken cancellationToken);

        Task<IList<UserDataDto>> GetUsersInRoleAsync(RoleEnum role, CancellationToken ct);

        Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);

        Task<Result> UpdateUserAsync(
           Guid userId,
           string firstName,
           string lastName,
           string email,
           RoleEnum role,
           string? password,
           CancellationToken cancellationToken);

        Task<Result<UserDataDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}