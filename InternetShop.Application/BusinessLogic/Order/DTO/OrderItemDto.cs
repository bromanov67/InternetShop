using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.BusinessLogic.Order.DTO
{
    public record OrderItemDto(string ProductId, int Quantity);
}
