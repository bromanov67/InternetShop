namespace InternetShop.Application.BusinessLogic.User.Interfaces
{
    public interface ITokenService
    {
        public string GenerateJwtToken(Domain.User user, CancellationToken cancellationToken);
    }
}
