using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Application.BusinessLogic.Order
{
    public class Order
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public DateTime CreatedAt { get; init; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; init; }
        public decimal TotalAmount { get; set; }
        public int PaymentTypeId { get; init; }
        public Order() { }
        public Order(Guid userId, int paymentTypeId, List<OrderItem> items)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            PaymentTypeId = paymentTypeId;
            Items = items;
            Status = OrderStatus.Created;
            CreatedAt = DateTime.UtcNow;
        }
        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }
        public void ProcessPayment()
        {
            if (Status != OrderStatus.Created)
                throw new InvalidOperationException("Order cannot be paid again.");

            Status = OrderStatus.Paid;
        }
    }
}
