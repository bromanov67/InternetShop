/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Database.Models
{
    public class CartEntity
    {
        public Guid UserId { get; set; } // Foreign key
        public UserEntity User { get; set; }
        public ICollection<CartItemEntity> Items { get; set; } = new List<CartItemEntity>();
    }
}
*/