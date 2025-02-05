using FluentResults;

namespace InternetShop.Application.User
{
    public interface IUserRepository
    {
        public Task CreateAsync(Domain.User user, CancellationToken cancellationToken);

        public Task<Result<List<Domain.User>>> GetAllAsync(CancellationToken cancellationToken);
     }
}
