namespace InternetShop.Database.Models
{
    public class PaymentEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public OrderEntity Order { get; set; } = null!;
        public int PaymentTypeId { get; set; }
        public PaymentTypeEntity PaymentType { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionId { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
