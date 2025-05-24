namespace InternetShop.Database.Models
{
    public class OrderEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StatusId { get; set; }
        public OrderStatusEntity Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }

        // Navigation properties
        public List<OrderItemEntity> Items { get; set; } = new();
        public PaymentEntity Payment { get; set; } = null!;
        public UserEntity? User { get; set; }
    }
}
