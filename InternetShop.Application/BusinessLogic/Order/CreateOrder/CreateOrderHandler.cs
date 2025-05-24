using FluentResults;
using InternetShop.Application.BusinessLogic.Order.DTO;
using InternetShop.Application.BusinessLogic.Order.Interfaces;
using InternetShop.Application.BusinessLogic.Product;
using MediatR;

namespace InternetShop.Application.BusinessLogic.Order.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICatalogRepository _catalogRepository;

        public CreateOrderHandler(
            IOrderRepository orderRepository,
            ICatalogRepository catalogRepository)
        {
            _orderRepository = orderRepository;
            _catalogRepository = catalogRepository;
        }

        public async Task<Result<OrderDto>> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Получаем актуальные цены из каталога
            var productPrices = await _catalogRepository.GetProductPricesAsync(
                request.Items.Select(i => i.ProductId.ToString()),
                cancellationToken);

            var productNames = await _catalogRepository.GetProductNamesAsync(
               request.Items.Select(i => i.ProductId.ToString()),
               cancellationToken);

            // 2. Проверяем наличие всех товаров
            var missingProducts = request.Items
                .Where(item => !productPrices.ContainsKey(item.ProductId.ToString()))
                .ToList();

            if (missingProducts.Any())
            {
                var missingIds = string.Join(", ", missingProducts.Select(x => x.ProductId));
                return Result.Fail<OrderDto>($"Товары не найдены: {missingIds}");
            }

            // 3. Создаем OrderItems с фиксированными ценами
            var orderItems = request.Items.Select(item =>
                new OrderItem(
                    item.ProductId,
                    productNames[item.ProductId.ToString()],
                    productPrices[item.ProductId.ToString()], // Фиксируем цену из каталога
                    item.Quantity)
            ).ToList();

            // 4. Создаем заказ
            var order = new Order(
                request.UserId,
                request.PaymentTypeId,
                orderItems);

            // 5. Сохраняем в репозитории
            var orderId = await _orderRepository.CreateOrderAsync(order, cancellationToken);

            if (orderId == Guid.Empty)
                return Result.Fail<OrderDto>("Не удалось создать заказ");

            // 6. Получаем полные данные заказа
            var createdOrder = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken);

            if (createdOrder is null)
                return Result.Fail<OrderDto>("Заказ не найден после создания");

            // 7. Маппим в DTO
            return Result.Ok(MapToDto(createdOrder));
        }

        private static OrderDto MapToDto(Order order) => new(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.Status,
            order.TotalAmount,
            order.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.Quantity)).ToList());
    }
}
