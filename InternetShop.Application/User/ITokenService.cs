using InternetShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.User
{
    public interface ITokenService
    {
        public string GenerateJwtToken(Domain.User user);
    }
}
