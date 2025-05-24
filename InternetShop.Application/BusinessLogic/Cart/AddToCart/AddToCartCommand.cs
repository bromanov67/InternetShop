using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.BusinessLogic.Cart.AddToCart
{
    public record AddToCartCommand(Guid UserId, string ProductId, int Quantity = 1);
}
