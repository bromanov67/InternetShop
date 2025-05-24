namespace InternetShop.Database.Models
{
    public class PaymentTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();
    }
}
