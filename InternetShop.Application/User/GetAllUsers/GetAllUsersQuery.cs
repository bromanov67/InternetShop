using FluentResults;
using InternetShop.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InternetShop.Application.User.GetAllUsers
{
    public record GetAllUsersQuery : IRequest<Result<List<Domain.User>>>
    {

    }
}
