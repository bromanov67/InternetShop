/*using InternetShop.Database.Models;
using InternetShop.Domain;

namespace InternetShop.Database.Services
{
    public static class UserExtensions
    {
        public static UserEntity ToEntity(this Domain.User user)
        {
            return new UserEntity
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };
        }

        public static User ToDomain(this UserEntity entity)
        {
            return new User
            {
                Id = entity.Id,
                Email = entity.Email,
                Firstname = entity.Firstname,
                Lastname = entity.Lastname,
                PasswordHash = entity.PasswordHash,
                Role = entity.Role
            };
        }
    }
}
*/