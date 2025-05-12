using Microsoft.AspNetCore.Identity;

namespace InternetShop.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public RoleEnum Role { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }
    }
}
