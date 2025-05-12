using InternetShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.BusinessLogic.User
{
    public interface IIdentityRepository
    {
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
        Task<Guid> CreateIdentityUserAsync(string email, string password, CancellationToken ct);
        Task AddToRoleAsync(Guid userId, RoleEnum role, CancellationToken ct);
        Task RemoveFromRoleAsync(Guid userId, RoleEnum role, CancellationToken ct);
        Task<bool> IsInRoleAsync(Guid userId, RoleEnum role, CancellationToken ct);
    }
}
