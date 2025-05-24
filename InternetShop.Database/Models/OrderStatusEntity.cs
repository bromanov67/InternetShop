using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Database.Models
{
    public class OrderStatusEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
    }
}
