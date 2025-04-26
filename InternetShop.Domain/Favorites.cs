using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Domain
{
    public class Favorites
    {
        public Guid Id { get; set; }

        public Guid userId { get; set; }

        public string ProductId { get; set; } = null!;
    }
}
