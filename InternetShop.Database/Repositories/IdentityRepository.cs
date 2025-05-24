using InternetShop.Database.DbContext;
using InternetShop.Database.Models;
using InternetShop.Database.Services;
using InternetShop.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Database.Repositories
{
    public class IdentityRepository :
         IUserStore<UserEntity>,
         IUserPasswordStore<UserEntity>,
         IUserEmailStore<UserEntity>

    {
        private readonly UserDbContext _context;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        public IdentityRepository(UserDbContext context, IPasswordHasher<UserEntity> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
        {
            return await _context.Users.AsNoTracking().AnyAsync(u => u.Email == email, ct);
        }

        public async Task<Guid> CreateUserAsync(string email, string password, User domainUser, CancellationToken ct)
        {
            // Теперь создаем UserEntity напрямую
            var userEntity = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(null, password),
                FirstName = domainUser.FirstName,
                LastName = domainUser.LastName,
                Role = domainUser.Role
            };

            await _context.Users.AddAsync(userEntity, ct);
            await _context.SaveChangesAsync(ct);

            return userEntity.Id;
        }


        public async Task RemoveFromRoleAsync(Guid userId, RoleEnum role, CancellationToken ct)
        {
            var roleName = RoleMapperService.ToName(role);
            var roleEntity = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName, ct);

            if (roleEntity == null) return;

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleEntity.Id, ct);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync(ct);
            }
        }

        public IQueryable<UserEntity> GetUsersQueryable()
        {
            return _context.Users.AsQueryable();
        }

        public async Task<bool> IsInRoleAsync(Guid userId, RoleEnum role, CancellationToken ct)
        {
            var roleName = RoleMapperService.ToName(role);
            return await _context.UserRoles
                .Join(_context.Roles,
                      ur => ur.RoleId,
                      r => r.Id,
                      (ur, r) => new { ur.UserId, r.Name })
                .AnyAsync(r => r.UserId == userId && r.Name == roleName, ct);
        }
        public async Task<bool> UpdateUserAsync(UserEntity user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
        public async Task AddToRoleAsync(Guid userId, RoleEnum role, CancellationToken ct)
        {
            var roleName = RoleMapperService.ToName(role);
            var roleEntity = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName, ct);
            if (roleEntity == null)
            {
                roleEntity = new IdentityRole<Guid>
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                await _context.Roles.AddAsync(roleEntity, ct);
                await _context.SaveChangesAsync(ct);
            }

            var existing = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleEntity.Id, ct);

            if (!existing)
            {
                await _context.UserRoles.AddAsync(new IdentityUserRole<Guid>
                {
                    UserId = userId,
                    RoleId = roleEntity.Id
                }, ct);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task RemoveFromRoleAsync(UserEntity user, string roleName, CancellationToken ct)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
            if (role == null) return;

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, ct);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync(ct);
            }
        }


        public async Task<IList<string>> GetRolesAsync(UserEntity user, CancellationToken ct)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync(ct);
        }

        public async Task<bool> IsInRoleAsync(UserEntity user, string roleName, CancellationToken ct)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
            if (role == null) return false;

            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, ct);
        }

        public async Task<IList<UserEntity>> GetUsersInRoleAsync(string roleName, CancellationToken ct)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
            if (role == null) return new List<UserEntity>();

            return await _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Join(_context.Users,
                      ur => ur.UserId,
                      u => u.Id,
                      (ur, u) => u)
                .ToListAsync(ct);
        }

        public IQueryable<UserEntity> Users =>
            _context.Users.AsNoTracking().Include(u => u.Id).AsQueryable();

        public async Task<IdentityResult> CreateAsync(UserEntity user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(UserEntity user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(UserEntity user, CancellationToken cancellationToken)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<UserEntity?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Id)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId, cancellationToken);
        }

        public async Task<UserEntity?> FindByEmailAsync(string email, CancellationToken ct)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public Task<string> GetNormalizedEmailAsync(UserEntity user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedEmail);

        public Task SetNormalizedEmailAsync(UserEntity user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(UserEntity user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(UserEntity user, CancellationToken cancellationToken) =>
            Task.FromResult(true); // или из поля, если используется

        public Task SetEmailConfirmedAsync(UserEntity user, bool confirmed, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task SetEmailAsync(UserEntity user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }
        public async Task<UserEntity?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
            await _context.Users.FirstOrDefaultAsync(u => u.UserName == normalizedUserName, cancellationToken);

        public Task<string> GetUserIdAsync(UserEntity user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Id.ToString());

        public Task<string?> GetUserNameAsync(UserEntity user, CancellationToken cancellationToken) =>
            Task.FromResult(user.UserName);

        public Task SetUserNameAsync(UserEntity user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedUserNameAsync(UserEntity user, CancellationToken cancellationToken) =>
            Task.FromResult(user.UserName.ToUpper());

        public Task SetNormalizedUserNameAsync(UserEntity user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<string?> GetPasswordHashAsync(UserEntity user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(UserEntity user, CancellationToken cancellationToken) =>
            Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

        public Task SetPasswordHashAsync(UserEntity user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public async Task<Guid> CreateIdentityUserAsync(string email, string password, CancellationToken ct)
        {
            var userEntity = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            // Хеширование пароля
            userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, password);

            // Сохранение
            await _context.Users.AddAsync(userEntity, ct);
            await _context.SaveChangesAsync(ct);

            return userEntity.Id;
        }
        public async Task<UserEntity?> GetUserByIdAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);
        }

        public async Task<UserWithRoles?> GetUserByIdWithRolesAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserWithRoles
                {
                    User = u,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == userId)
                        .Join(_context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name)
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public class UserWithRoles
        {
            public UserEntity User { get; set; }
            public List<string> Roles { get; set; }
        }
        public void Dispose() { }

    }
}