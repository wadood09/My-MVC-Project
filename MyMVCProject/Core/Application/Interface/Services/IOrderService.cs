using MyMVCProject.Models;
using MyMVCProject.Models.OrderModel;

namespace MyMVCProject.Core.Application.Interface.Services
{
    public interface IOrderService
    {
        Task<BaseResponse> CreateOrder(OrderRequest request);
        Task<BaseResponse> CreateOrderItem(OrderItemRequest request);
        Task<BaseResponse<ICollection<OrdersResponse>>> GetAllOrders();
        Task<BaseResponse<ICollection<OrdersResponse>>> GetOrdersByStatus(int status);
        Task<BaseResponse<ICollection<CustomersOrderResponse>>> GetOrdersByStatusAndCustomer(int id, int status);
        Task<BaseResponse<ICollection<CustomersOrderResponse>>> GetAllOrdersByCustomer(int customerId);
        Task<BaseResponse<OrderResponse>> GetOrder(int id);
        Task<BaseResponse<OrderItemResponse>> GetOrderItem(Guid id);
        Task<BaseResponse> UpdateOrder(int id, UpdateOrderRequest request);
        Task<BaseResponse> UpdateOrderItem(Guid id, UpdateOrderItemRequest request);
        Task<BaseResponse> DeleteOrder(int id);
        Task<BaseResponse> DeleteOrderItem(Guid id);
        Task<BaseResponse<OrderResponse>> GetCurrentOrder();
        Task<BaseResponse> CancelOrder(int id);
        Task<BaseResponse> MakeOrder();
        Task<BaseResponse<CartModel>> GetCart();
        Task<int> LoggedinCustomerOrderCount();
        Task<int> ChangeOrderItemQuantity(Guid id, int sign);
        Task<BaseResponse> VerifyOrder(int id);
    }
}
