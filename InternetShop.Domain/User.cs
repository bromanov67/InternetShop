using Microsoft.AspNetCore.Identity;

namespace InternetShop.Domain
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
        
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        //public string? RefreshToken { get; set; }
        //public DateTime RefreshTokenExpiryTime { get; set; }


        //perhabs to delete 
        public User() { }

        public User(string firstname, string lastname, string email)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
        }
    }
}
