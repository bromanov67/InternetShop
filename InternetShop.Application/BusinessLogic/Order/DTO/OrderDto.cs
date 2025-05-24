namespace InternetShop.Application.BusinessLogic.Order.DTO
{
    public record OrderDto(
     Guid Id,
     Guid UserId,
     DateTime CreatedAt,
     OrderStatus Status,
     decimal TotalAmount,
     List<OrderItemDto> Items);
}