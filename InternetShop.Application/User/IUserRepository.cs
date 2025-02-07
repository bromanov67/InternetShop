using FluentResults;
using InternetShop.Domain;

namespace InternetShop.Application.User
{
    public interface IUserRepository
    {
        public Task CreateAsync(Domain.User user, CancellationToken cancellationToken);

        public Task<Result<List<Domain.User>>> GetAllAsync(CancellationToken cancellationToken);

        public Task<Domain.User> GetByEmailAsync(string email, CancellationToken cancellationToken);

        public Task AddAsync(Domain.User user);
    }
}
