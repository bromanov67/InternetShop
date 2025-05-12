using FluentResults;
using InternetShop.Domain;

namespace InternetShop.Application.BusinessLogic.User
{
    public interface IUserRepository
    {
        //Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
        //Task<Guid> CreateUserAsync(Domain.User user, string password, CancellationToken ct);
        //Task UpdateUserAsync(Domain.User user, Guid userId, CancellationToken ct);
        //Task<Domain.User?> GetUserByIdAsync(Guid id, CancellationToken ct);
        //Task<Domain.User?> GetUserByEmailAsync(string email, CancellationToken ct);
        //Task AddToRoleAsync(Guid userId, RoleEnum role, CancellationToken ct);
        //Task RemoveFromRoleAsync(Guid userId, RoleEnum role, CancellationToken ct);
        //Task<bool> IsInRoleAsync(Guid userId, RoleEnum role, CancellationToken ct);
        Task<Guid> CreateUserAsync(Domain.User user, string password, CancellationToken ct);
        Task UpdateUserAsync(Domain.User user, Guid userId, CancellationToken ct);
        Task<Domain.User?> GetUserByIdAsync(Guid id, CancellationToken ct);
        Task<Domain.User?> GetUserByEmailAsync(string email, CancellationToken ct);
    }
}
