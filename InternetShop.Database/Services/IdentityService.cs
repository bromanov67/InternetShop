using FluentResults;
using InternetShop.Application.BusinessLogic;
using InternetShop.Application.BusinessLogic.User;
using InternetShop.Application.BusinessLogic.User.GetAllUsers;
using InternetShop.Database.Models;
using InternetShop.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InternetShop.Database.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly ILogger<IdentityService> _logger;
        private readonly IdentityRepository _identityRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        public IdentityService(
            ITokenService tokenService,
            ILogger<IdentityService> logger,
            IdentityRepository identityRepository,
            IPasswordHasher<UserEntity> passwordHasher)
        {
            _tokenService = tokenService;
            _logger = logger;
            _identityRepository = identityRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<string>> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Находим пользователя по email
                var userEntity = await _identityRepository.FindByEmailAsync(email, cancellationToken);
                if (userEntity == null)
                {
                    _logger.LogWarning("Login failed: user with email {Email} not found", email);
                    return Result.Fail<string>("Invalid login attempt");
                }

                // 2. Проверяем пароль
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
                    userEntity,
                    userEntity.PasswordHash,
                    password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Invalid password for user {Email}", email);
                    return Result.Fail<string>("Invalid login attempt");
                }

                // 3. Получаем роли пользователя (если нужно для токена)
                var roles = await _identityRepository.GetRolesAsync(userEntity, cancellationToken);
                var mainRole = roles.FirstOrDefault();

                // 4. Создаем объект пользователя для генерации токена
                var user = new User
                {
                    Id = userEntity.Id,
                    Email = userEntity.Email,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    Role = !string.IsNullOrEmpty(mainRole)
                        ? RoleMapperService.FromString(mainRole)
                        : RoleEnum.Client // Значение по умолчанию
                };

                // 5. Генерируем JWT токен
                var token = _tokenService.GenerateJwtToken(user, cancellationToken);

                _logger.LogInformation("User {Email} successfully logged in", email);
                return Result.Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", email);
                return Result.Fail<string>("Login failed").WithError(ex.Message);
            }
        }
        public async Task<Result<Guid>> RegisterAsync(string email, string password, User domainUser, CancellationToken ct)
        {
            try
            {
                // Проверка существования пользователя
                if (await _identityRepository.ExistsByEmailAsync(email, ct))
                {
                    return Result.Fail<Guid>("Email already exists");
                }

                // Создание пользователя (DomainUser больше не сохраняется отдельно)
                var userId = await _identityRepository.CreateUserAsync(email, password, domainUser, ct);

                // Добавление роли
                await _identityRepository.AddToRoleAsync(userId, domainUser.Role, ct);

                return Result.Ok(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return Result.Fail<Guid>("Registration failed");
            }
        }

        public async Task<Result<Guid>> ValidateCredentialsAsync(
       string email,
       string password,
       CancellationToken ct)
        {
            try
            {
                var userEntity = await _identityRepository.FindByEmailAsync(email, ct);
                if (userEntity == null)
                    return Result.Fail<Guid>("Invalid credentials");

                if (_passwordHasher.VerifyHashedPassword(
                    userEntity,
                    userEntity.PasswordHash,
                    password) != PasswordVerificationResult.Success)
                {
                    return Result.Fail<Guid>("Invalid credentials");
                }

                return Result.Ok(userEntity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication error");
                return Result.Fail<Guid>("Authentication failed");
            }
        }

        public async Task<Result<string>> GetUserEmailAsync(Guid userId, CancellationToken ct)
        {
            var user = await _identityRepository.GetUserByIdAsync(userId, ct);
            return user?.Email != null
                ? Result.Ok(user.Email)
                : Result.Fail<string>("User email not found");
        }
        public async Task<Result<(string FirstName, string LastName)>> GetUserNamesAsync(Guid userId, CancellationToken ct)
        {
            var user = await _identityRepository.GetUserByIdAsync(userId, ct);
            return user != null
                ? Result.Ok((user.FirstName, user.LastName))
                : Result.Fail<(string, string)>("User names not found");
        }

        public async Task<Result<string>> GetUserRoleAsync(Guid userId, CancellationToken ct)
        {
            var user = await _identityRepository.GetUserByIdWithRolesAsync(userId, ct);
            return user?.Roles?.FirstOrDefault() != null
                ? Result.Ok(user.Roles.First())
                : Result.Ok("User"); // Роль по умолчанию
        }

        public async Task<Result<(UserEntity User, string Role)>> GetUserWithRoleAsync(
            Guid userId,
            CancellationToken ct)
        {
            var user = await _identityRepository.GetUserByIdWithRolesAsync(userId, ct);

            if (user?.User == null)
                return Result.Fail<(UserEntity, string)>("User not found");

            var role = user.Roles.FirstOrDefault() ?? "User";

            return Result.Ok((user.User, role));
        }

        public async Task<Result<CurrentUserDto>> GetCurrentUserDataAsync(Guid userId, CancellationToken ct)
        {
            try
            {
                // 1. Получаем базовые данные пользователя (используем внутренний UserDto)
                var user = await _identityRepository.GetUserByIdAsync(userId, ct);
                if (user == null)
                    return Result.Fail<CurrentUserDto>("User not found");

                // 2. Получаем роль пользователя
                var role = await GetUserRoleAsync(userId, ct) ?? "User";

                // 3. Преобразуем во внешний DTO
                return Result.Ok(new CurrentUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = role.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user data");
                return Result.Fail<CurrentUserDto>("Error retrieving user data");
            }
        }

        public async Task<List<UserDataDto>> GetAllUsersAsync(
        List<RoleEnum>? roleFilters,
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
        {
            // Валидация параметров
            if (pageNumber < 1) throw new ArgumentException("Page number must be positive");
            if (pageSize < 1 || pageSize > 100) throw new ArgumentException("Page size must be between 1 and 100");

            var query = _identityRepository.GetUsersQueryable();

            // Фильтрация по ролям
            if (roleFilters?.Any() == true)
            {
                query = query.Where(u => roleFilters.Contains(u.Role));
            }

            // Поиск (безопасная обработка null)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim();
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(term)) ||
                    (u.LastName != null && u.LastName.Contains(term)) ||
                    (u.Email != null && u.Email.Contains(term)));
            }

            // Пагинация
            var users = await query
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDataDto(
                    u.Id,
                    u.Email ?? string.Empty,
                    u.FirstName ?? string.Empty,
                    u.LastName ?? string.Empty,
                    u.Role))
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Retrieved {Count} users", users.Count);
            return users;
        }

        public async Task<IList<UserDataDto>> GetUsersInRoleAsync(RoleEnum role, CancellationToken ct)
        {
            var roleName = RoleMapperService.ToName(role);
            var userEntities = await _identityRepository.GetUsersInRoleAsync(roleName, ct);

            // Ручное преобразование Entity → DTO
            return userEntities.Select(user => new UserDataDto(
                Id: user.Id,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email,
                Role: role
            )).ToList();
        }

        public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _identityRepository.DeleteUserAsync(userId, cancellationToken);
                return result ? Result.Ok() : Result.Fail("User not found");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<Result> UpdateUserAsync(
            Guid userId,
            string firstName,
            string lastName,
            string email,
            RoleEnum role,
            string? password,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _identityRepository.GetUserByIdAsync(userId, cancellationToken);
                if (user == null)
                    return Result.Fail("User not found");

                // Обновляем данные
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.Role = role;

                // Обновляем пароль, если он указан
                if (!string.IsNullOrEmpty(password))
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, password);
                }

                var success = await _identityRepository.UpdateUserAsync(user, cancellationToken);
                return success ? Result.Ok() : Result.Fail("Failed to update user");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error updating user: {ex.Message}");
            }
        }

        public async Task<Result<UserDataDto>> GetUserByIdAsync(        
            Guid userId,
            CancellationToken cancellationToken)
        {
            var user = await _identityRepository.GetUserByIdAsync(userId, cancellationToken);

            if (user == null)
                return Result.Fail<UserDataDto>("User not found");

            return Result.Ok(new UserDataDto(
                Id: user.Id,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email,
                Role: user.Role
            ));
        }
    }
}
