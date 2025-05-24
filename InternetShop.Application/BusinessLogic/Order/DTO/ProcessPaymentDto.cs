namespace InternetShop.Application.BusinessLogic.Order.DTO
{
    public record ProcessPaymentDto(Guid OrderId, decimal Amount);
}
