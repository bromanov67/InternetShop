using Microsoft.AspNetCore.Identity;

namespace InternetShop.Domain
{
    public class User : IdentityUser
    {
        public Guid Id { get; set; }
        
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public User() { }

        public User(string firstname, string lastname, string email)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
        }
    }
}
