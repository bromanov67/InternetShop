using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Domain
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public DateTime DateOfOrder { get; set; }
    }
}
