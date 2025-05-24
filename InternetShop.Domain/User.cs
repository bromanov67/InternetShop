using InternetShop.Application.BusinessLogic.Order;
using Microsoft.AspNetCore.Identity;

namespace InternetShop.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public RoleEnum Role { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public Cart Cart { get; } = new();


        private List<Order> _orders = new();

        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    }
}
