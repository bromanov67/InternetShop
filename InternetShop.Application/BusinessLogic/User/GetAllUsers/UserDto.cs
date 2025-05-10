namespace InternetShop.Application.BusinessLogic.User.GetAllUsers
{
    public record UserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; }
        public string Firstname { get; init; }
        public string Lastname { get; init; }
    }
}
