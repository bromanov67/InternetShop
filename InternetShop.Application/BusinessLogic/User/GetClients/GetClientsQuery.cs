using InternetShop.Application.BusinessLogic.User.GetAllUsers;
using InternetShop.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.BusinessLogic.User.GetClients
{
    public class GetClientsQuery : IRequest<IEnumerable<UserDataDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
