namespace InternetShop.Database.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }

        public string Firstname { get; set; } = string.Empty;

        public string Lastname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
    }
}
