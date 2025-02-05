using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.User.GetAllUsers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, Result<List<Domain.User>>>
    {
        private IUserRepository _userRepository;
        public GetAllUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<List<Domain.User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var allUsers = await _userRepository.GetAllAsync(cancellationToken);

            return allUsers;
        }
    }
}
