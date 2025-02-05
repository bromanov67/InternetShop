using FluentResults;
using InternetShop.Application.User;
using InternetShop.Database.Models;
using InternetShop.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
