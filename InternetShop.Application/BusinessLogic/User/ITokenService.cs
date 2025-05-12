namespace InternetShop.Application.BusinessLogic.User
{
    public interface ITokenService
    {
        public string GenerateJwtToken(Domain.User user, CancellationToken cancellationToken);
    }
}
