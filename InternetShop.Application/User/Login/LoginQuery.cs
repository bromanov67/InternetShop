using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.User.Login
{
    public record LoginQuery(string Email, string Password) : IRequest<Result<string>>;
}
