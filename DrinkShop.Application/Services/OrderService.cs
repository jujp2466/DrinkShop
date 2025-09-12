using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;

namespace DrinkShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            var success = await _orderRepository.CreateOrderAsync(dto);
            if (!success)
            {
                throw new System.Exception("Order creation failed in repository.");
            }
            // WARNING: This is not a robust way to get the created order.
            // The repository should ideally return the created entity or its ID.
            var orders = await _orderRepository.GetOrdersByUserIdAsync(dto.UserId);
            var createdOrder = orders.OrderByDescending(o => o.Id).FirstOrDefault();
            if (createdOrder == null)
            {
                 throw new System.Exception("Could not retrieve the newly created order.");
            }
            return createdOrder;
        }

        public async Task<IEnumerable<OrderDto>> GetAsync(int? userId, string? role)
        {
            if (role == "admin")
            {
                return await _orderRepository.GetAllOrdersAsync();
            }
            
            if (userId == null) return new List<OrderDto>();
            return await _orderRepository.GetOrdersByUserIdAsync(userId.Value);
        }

        public async Task<OrderDto?> UpdateStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return null;
            order.Status = status;
            // TODO: Implement an update method in the repository.
            // await _orderRepository.UpdateAsync(order);
            return order;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // TODO: Implement a delete method in the repository.
            // return await _orderRepository.DeleteAsync(id);
            return await Task.FromResult(false);
        }
    }
}
