using FluentResults;
using InternetShop.Application.BusinessLogic.User.DTO;
using InternetShop.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Application.BusinessLogic.User.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<List<UserDataDto>> 
    {
        // Фильтр по ролям (0 - Admin, 1 - Manager, 2 - Employee, 3 - Client)
        public List<RoleEnum>? RoleFilters { get; set; }

        // Поисковая строка (по имени, фамилии или email)
        public string? SearchTerm { get; set; }

        // Номер страницы (для пагинации)
        public int PageNumber { get; set; } = 1;

        // Размер страницы (для пагинации)
        public int PageSize { get; set; } = 20;
    }
}
