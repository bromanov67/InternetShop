using FluentResults;
using InternetShop.Application.User;
using InternetShop.Database.Models;
using InternetShop.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace InternetShop.Database
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateAsync(User user, CancellationToken cancellationToken)
        {
            var userEntity = new UserEntity
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email

            };
            _dbContext.Set<UserEntity>().Add(userEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Result<List<User>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var userEntities = await _dbContext.Set<UserEntity>().ToListAsync();
            var allUsers =  userEntities.Select(u => new User
            {
                Id = u.Id,
                Firstname = u.Firstname,
                Lastname = u.Lastname,
                Email = u.Email
            }).ToList();

            return Result.Ok(allUsers);
        }

        public async Task AddAsync(User user)
        {

            var userEntity = new UserEntity
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                PasswordHash = user.PasswordHash
            };

            await _dbContext.Users.AddAsync(userEntity);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var userEntity = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (userEntity == null)
            {
                return null;
            }

            return new User
            {
                Id = userEntity.Id,
                Email = userEntity.Email,
                Firstname = userEntity.Firstname,
                Lastname = userEntity.Lastname,
                PasswordHash = userEntity.PasswordHash
            };
        }
    }
}
