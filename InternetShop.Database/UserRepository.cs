using InternetShop.Application.User;
using InternetShop.Database.Models;
using InternetShop.Domain;
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
    }
}
