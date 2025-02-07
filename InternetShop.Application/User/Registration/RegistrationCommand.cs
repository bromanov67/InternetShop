using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.User.Registration
{
    public record RegistrationCommand(string Fistname, string Lastname, string Email, string Password) : IRequest<IActionResult>;
}
