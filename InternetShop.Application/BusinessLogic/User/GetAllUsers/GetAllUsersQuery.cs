using FluentResults;
using InternetShop.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Application.BusinessLogic.User.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<List<UserDataDto>> 
    {
        /// <summary>
        /// Фильтр по ролям (0 - Admin, 1 - Manager, 2 - Employee, 3 - Client)
        /// </summary>
        public List<RoleEnum>? RoleFilters { get; set; }

        /// <summary>
        /// Поисковая строка (по имени, фамилии или email)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Номер страницы (для пагинации)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Размер страницы (для пагинации)
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
