using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Core.Domain.Enums;
using MyMVCProject.Models;
using MyMVCProject.Models.OrderModel;
using System.Security.Claims;

namespace MyMVCProject.Core.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFoodRepository _foodRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMenuRepository _menuRepository;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IFoodRepository foodRepository, IHttpContextAccessor httpContext, ICustomerRepository customerRepository, IWalletRepository walletRepository, IOrderItemRepository orderItemRepository, IMenuRepository menuRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _foodRepository = foodRepository;
            _httpContext = httpContext;
            _customerRepository = customerRepository;
            _walletRepository = walletRepository;
            _orderItemRepository = orderItemRepository;
            _menuRepository = menuRepository;
        }

        public async Task<BaseResponse> CreateOrder(OrderRequest request)
        {
            var customer = await _customerRepository.GetAsync(request.CustomerId);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Message = $"Customer with id '{request.CustomerId}' does not exists",
                    IsSuccessful = false
                };
            }

            var items = new List<OrderItem>();
            foreach (var item in request.Items)
            {
                var food = await _foodRepository.GetAsync(item.FoodId);
                if (food == null)
                {
                    return new BaseResponse
                    {
                        Message = $"Food chosen does not exists",
                        IsSuccessful = false
                    };
                }

                if (item.Quantity > food.Quantity)
                {
                    return new BaseResponse
                    {
                        Message = $"The quantity of {food.Name} ordered is greater than the remaining quantity which is {food.Quantity}",
                        IsSuccessful = false
                    };
                }

                food.Quantity -= item.Quantity;
                items.Add(new OrderItem
                {
                    FoodName = food.Name,
                    FoodId = food.Id,
                    UnitPrice = food.Price,
                    Units = item.Quantity
                });

                _foodRepository.Update(food);
            }
            items.ForEach(x => _orderItemRepository.AddAsync(x));

            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var order = new Order
            {
                CustomerId = request.CustomerId,
                Customer = customer,
                CreatedBy = loginUserId,
                Description = request.Description,
                Items = items
            };

            customer.Orders.Add(order);
            _customerRepository.Update(customer);
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Order created successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> CreateOrderItem(OrderItemRequest request)
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var customer = await _customerRepository.GetAsync(x => x.User.Email == loginUserEmail);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Message = $"Customer with email '{loginUserEmail}' does not exists",
                    IsSuccessful = false
                };
            }

            var order = await _orderRepository.GetAsync(x => x.OrderStatus == Status.Pending && x.Customer == customer);
            if (order == null)
            {
                order = new Order
                {
                    CreatedBy = customer.Id.ToString(),
                    CustomerId = customer.Id,
                    Customer = customer
                };
                await _orderRepository.AddAsync(order);
                customer.Orders.Add(order);
            }

            var food = await _foodRepository.GetAsync(request.FoodId);
            if (food == null)
            {
                return new BaseResponse
                {
                    Message = $"Food chosen does not exists",
                    IsSuccessful = false
                };
            }

            var exists = order.Items.Any(x => x.FoodId == request.FoodId);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Item has already been added to cart",
                    IsSuccessful = false
                };
            }

            if (request.Quantity > food.Quantity)
            {
                return new BaseResponse
                {
                    Message = $"The quantity of {food.Name} ordered is greater than the remaining quantity which is {food.Quantity}",
                    IsSuccessful = false
                };
            }

            food.Quantity -= request.Quantity;
            await _orderItemRepository.AddAsync(new OrderItem
            {
                FoodName = food.Name,
                FoodId = food.Id,
                UnitPrice = food.Price,
                Units = request.Quantity,
                OrderId = order.Id,
                Order = order
            });

            _foodRepository.Update(food);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = $"{food.Name} successfully added to cart",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> DeleteOrder(int id)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return new BaseResponse
                {
                    Message = "Order does not exists",
                    IsSuccessful = false
                };
            }

            _orderRepository.Remove(order);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Order successfully deleted",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> DeleteOrderItem(Guid id)
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var customer = await _customerRepository.GetAsync(x => x.User.Email == loginUserEmail);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Message = $"Customer with email '{loginUserEmail}' does not exists",
                    IsSuccessful = false
                };
            }

            var order = await _orderRepository.GetAsync(x => x.OrderStatus == Status.Pending && x.Customer == customer);
            if (order == null)
            {
                return new BaseResponse
                {
                    Message = "Order does not exists",
                    IsSuccessful = false
                };
            }

            var orderItem = await _orderItemRepository.GetAsync(id);
            if (orderItem == null)
            {
                return new BaseResponse
                {
                    Message = "Order item does not exists",
                    IsSuccessful = false
                };
            }

            var food = await _foodRepository.GetAsync(orderItem.FoodId);
            if (food == null)
            {
                return new BaseResponse
                {
                    Message = "Food does not exists",
                    IsSuccessful = false
                };
            }
            food.Quantity += orderItem.Units;

            _foodRepository.Update(food);
            _orderItemRepository.Remove(orderItem);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = $"{food.Name} successfully removed from cart",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse<ICollection<OrdersResponse>>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            return new BaseResponse<ICollection<OrdersResponse>>
            {
                Message = "List of orders",
                IsSuccessful = true,
                Value = orders.SkipWhile(x => x.OrderStatus == Status.Pending).Select(x => new OrdersResponse
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.User.FirstName + " " + x.Customer.User.LastName,
                    DateCreated = x.DateCreated,
                    TotalPrice = x.Items.Select(x => x.UnitPrice * x.Units).Sum(),
                    Description = x.Description ?? "This order has no description",
                    Status = x.OrderStatus
                }).ToList()
            };
        }

        public async Task<BaseResponse<ICollection<CustomersOrderResponse>>> GetAllOrdersByCustomer(int customerId)
        {
            var orders = await _orderRepository.GetAllAsync(x => x.CustomerId == customerId);
            if (orders.Count == 0)
            {
                return new BaseResponse<ICollection<CustomersOrderResponse>>
                {
                    Message = "No order has yet to be made by this customer",
                    IsSuccessful = false
                };
            }

            return new BaseResponse<ICollection<CustomersOrderResponse>>
            {
                Message = "Orders made by customer",
                IsSuccessful = true,
                Value = orders.SkipWhile(x => x.OrderStatus == Status.Pending).Select(x => new CustomersOrderResponse
                {
                    Id = x.Id,
                    DateCreated = x.DateCreated,
                    Description = x.Description ?? "This order has no description",
                    TotalPrice = x.Items.Select(x => x.UnitPrice * x.Units).Sum(),
                    Status = x.OrderStatus
                }).ToList()
            };
        }

        public async Task<BaseResponse<OrderResponse>> GetOrder(int id)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return new BaseResponse<OrderResponse>
                {
                    Message = "Order not found",
                    IsSuccessful = false
                };
            }

            return new BaseResponse<OrderResponse>
            {
                Message = "Order successfully found",
                IsSuccessful = true,
                Value = new OrderResponse
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer.User.FirstName + " " + order.Customer.User.LastName,
                    DateCreated = order.DateCreated,
                    TotalPrice = order.Items.Select(x => x.UnitPrice * x.Units).Sum(),
                    Description = order.Description ?? "This order has no description",
                    Items = order.Items.Select(x => new OrderItemResponse
                    {
                        Id = x.Id,
                        FoodId = x.FoodId,
                        FoodName = x.FoodName,
                        UnitPrice = x.UnitPrice,
                        Units = x.Units
                    }).ToList(),
                    OrderStatus = order.OrderStatus
                }
            };
        }

        public async Task<BaseResponse<OrderResponse>> GetCurrentOrder()
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var order = await _orderRepository.GetAsync(x => x.OrderStatus == Status.Pending && x.Customer.User.Email == loginUserEmail);
            if (order == null)
            {
                return new BaseResponse<OrderResponse>
                {
                    Message = "Order not found",
                    IsSuccessful = false
                };
            }

            return new BaseResponse<OrderResponse>
            {
                Message = "Order successfully found",
                IsSuccessful = true,
                Value = new OrderResponse
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer.User.FirstName + " " + order.Customer.User.LastName,
                    DateCreated = order.DateCreated,
                    TotalPrice = order.Items.Select(x => x.UnitPrice * x.Units).Sum(),
                    Description = order.Description ?? "This order has no description",
                    Items = order.Items.Select(x => new OrderItemResponse
                    {
                        Id = x.Id,
                        FoodId = x.FoodId,
                        FoodName = x.FoodName,
                        UnitPrice = x.UnitPrice,
                        Units = x.Units
                    }).ToList(),
                    OrderStatus = order.OrderStatus
                }
            };
        }

        public async Task<BaseResponse<OrderItemResponse>> GetOrderItem(Guid id)
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var order = await _orderRepository.GetAsync(x => x.OrderStatus == Status.Pending && x.Customer.User.Email == loginUserEmail);
            if (order == null)
            {
                return new BaseResponse<OrderItemResponse>
                {
                    Message = "Order does not exists",
                    IsSuccessful = false
                };
            }

            var orderItem = await _orderItemRepository.GetAsync(id);
            if (orderItem == null)
            {
                return new BaseResponse<OrderItemResponse>
                {
                    Message = "Order item does not exists",
                    IsSuccessful = false
                };
            }

            return new BaseResponse<OrderItemResponse>
            {
                Message = "Order item successfully found",
                IsSuccessful = true,
                Value = new OrderItemResponse
                {
                    Id = orderItem.Id,
                    FoodId = orderItem.FoodId,
                    FoodName = orderItem.FoodName,
                    UnitPrice = orderItem.UnitPrice,
                    Units = orderItem.Units
                }
            };
        }

        public async Task<BaseResponse<ICollection<OrdersResponse>>> GetOrdersByStatus(int status)
        {
            var orders = await _orderRepository.GetAllAsync();
            var statusOrders = orders.Where(x => (int)x.OrderStatus == status).ToList();
            if (statusOrders.Count == 0)
            {
                return new BaseResponse<ICollection<OrdersResponse>>
                {
                    Message = $"No orders with status {(Status)status}",
                    IsSuccessful = false
                };
            }

            return new BaseResponse<ICollection<OrdersResponse>>
            {
                Message = $"All orders with status {(Status)status}",
                IsSuccessful = true,
                Value = statusOrders.Select(x => new OrdersResponse
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.User.FirstName + " " + x.Customer.User.LastName,
                    DateCreated = x.DateCreated,
                    TotalPrice = x.Items.Select(x => x.UnitPrice * x.Units).Sum(),
                    Description = x.Description ?? "This order has no description",
                    Status = x.OrderStatus
                }).ToList()
            };
        }

        public async Task<BaseResponse<ICollection<CustomersOrderResponse>>> GetOrdersByStatusAndCustomer(int id, int status)
        {
            var orders = await _orderRepository.GetAllAsync(x => x.CustomerId == id);
            var statusOrders = orders.Where(x => (int)x.OrderStatus == status).ToList();
            if (statusOrders.Count == 0)
            {
                return new BaseResponse<ICollection<CustomersOrderResponse>>
                {
                    Message = $"No orders with status {(Status)status}",
                    IsSuccessful = false
                };
            }

            return new BaseResponse<ICollection<CustomersOrderResponse>>
            {
                Message = $"Customer orders with status {(Status)status}",
                IsSuccessful = true,
                Value = orders.Select(x => new CustomersOrderResponse
                {
                    Id = x.Id,
                    DateCreated = x.DateCreated,
                    Description = x.Description ?? "This order has no description",
                    TotalPrice = x.Items.Select(x => x.UnitPrice * x.Units).Sum(),
                    Status = x.OrderStatus
                }).ToList()
            };
        }

        public async Task<BaseResponse> UpdateOrder(int id, UpdateOrderRequest request)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return new BaseResponse
                {
                    Message = "Order does not exists",
                    IsSuccessful = false
                };
            }

            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            order.Description = request.Description ?? order.Description;
            order.OrderStatus = request.OrderStatus ?? order.OrderStatus;
            order.DateModified = DateTime.Now;
            order.ModifiedBy = loginUserId;

            _orderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                IsSuccessful = true,
                Message = "Order successfully updated",
            };
        }

        public async Task<BaseResponse> UpdateOrderItem(Guid id, UpdateOrderItemRequest request)
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var order = await _orderRepository.GetAsync(x => x.OrderStatus == Status.Pending && x.Customer.User.Email == loginUserEmail);
            if (order == null)
            {
                return new BaseResponse
                {
                    Message = "Order does not exists",
                    IsSuccessful = false
                };
            }

            var orderItem = await _orderItemRepository.GetAsync(id);
            if (orderItem == null)
            {
                return new BaseResponse
                {
                    Message = "Order item does not exists",
                    IsSuccessful = false
                };
            }


            var food = await _foodRepository.GetAsync(orderItem.FoodId);
            if (request.FoodId.HasValue)
            {
                food.Quantity += orderItem.Units;
                _foodRepository.Update(food);

                food = await _foodRepository.GetAsync(request.FoodId.Value);
                if (food == null)
                {
                    return new BaseResponse
                    {
                        Message = $"Food chosen does not exists",
                        IsSuccessful = false
                    };
                }

                var exists = order.Items.Any(x => x.FoodId == request.FoodId);
                if (exists)
                {
                    return new BaseResponse
                    {
                        Message = "Order item already exists",
                        IsSuccessful = false
                    };
                }

                if (request.Quantity.HasValue)
                {
                    if (request.Quantity.Value > food.Quantity)
                    {
                        return new BaseResponse
                        {
                            Message = $"The quantity of {food.Name} ordered is greater than the remaining quantity which is {food.Quantity}",
                            IsSuccessful = false
                        };
                    }
                }

                if (orderItem.Units > food.Quantity)
                {
                    return new BaseResponse
                    {
                        Message = $"The quantity of {food.Name} ordered is greater than the remaining quantity which is {food.Quantity}",
                        IsSuccessful = false
                    };
                }
            }

            if (request.Quantity.HasValue)
            {
                if (request.Quantity.Value > food.Quantity)
                {
                    return new BaseResponse
                    {
                        Message = $"The quantity of {food.Name} ordered is greater than the remaining quantity which is {food.Quantity}",
                        IsSuccessful = false
                    };
                }
                food.Quantity -= request.Quantity.Value;
                _foodRepository.Update(food);
            }

            orderItem.UnitPrice = food.Price;
            orderItem.Units = request.Quantity ?? orderItem.Units;
            orderItem.FoodId = request.FoodId ?? orderItem.FoodId;
            orderItem.FoodName = food.Name;

            _orderItemRepository.Update(orderItem);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Order item successfully updated",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> MakeOrder()
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            var customer = await _customerRepository.GetAsync(x => x.User.Email == loginUserEmail);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Message = $"Only customers are allowed to make order",
                    IsSuccessful = false
                };
            }

            var order = await _orderRepository.GetAsync(x => x.OrderStatus == Status.Pending && x.Customer.User.Email == loginUserEmail);
            if (order == null || order.Items.Count == 0)
            {
                return new BaseResponse
                {
                    Message = $"No item has been added to cart",
                    IsSuccessful = false
                };
            }

            decimal totalPrice = order.Items.Select(x => x.UnitPrice * x.Units).Sum();

            if (totalPrice > customer.Wallet.Balance)
            {
                return new BaseResponse
                {
                    Message = $"Insufficient wallet balance",
                    IsSuccessful = false
                };
            }

            order.OrderStatus = Status.Processing;
            customer.Wallet.Balance -= totalPrice;
            _walletRepository.Update(customer.Wallet);

            _orderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Order is being processed",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> CancelOrder(int id)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return new BaseResponse
                {
                    Message = "Order does not exists",
                    IsSuccessful = false
                };
            }

            if (order.OrderStatus != Status.Processing)
            {
                return new BaseResponse
                {
                    Message = "Only processing orders can be cancelled",
                    IsSuccessful = false
                };
            }

            var customer = await _customerRepository.GetAsync(x => x.Id == order.CustomerId);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Message = $"Customer acount does not exists",
                    IsSuccessful = false
                };
            }

            decimal totalPrice = order.Items.Select(x => x.UnitPrice * x.Units).Sum();

            order.OrderStatus = Status.Cancelled;
            customer.Wallet.Balance += totalPrice;

            _walletRepository.Update(customer.Wallet);
            _orderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Order has been successfully cancelled",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> VerifyOrder(int id)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return new BaseResponse
                {
                    Message = $"Order does not exists",
                    IsSuccessful = false
                };
            }

            var customer = await _customerRepository.GetAsync(x => x.Id == order.CustomerId);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Message = $"Customer acount does not exists",
                    IsSuccessful = false
                };
            }

            decimal totalPrice = order.Items.Select(x => x.UnitPrice * x.Units).Sum();

            var wallet = await _walletRepository.GetAsync(1);
            wallet.Balance += totalPrice;

            order.OrderStatus = Status.Successfull;
            _walletRepository.Update(wallet);

            _orderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Order verification successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse<CartModel>> GetCart()
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var order = await _orderRepository.GetAsync(x => x.OrderStatus == Status.Pending && x.Customer.User.Email == loginUserEmail);
            if (order == null || order.Items.Count == 0)
            {
                return new BaseResponse<CartModel>
                {
                    Message = "Your cart is empty",
                    IsSuccessful = false
                };
            }

            var menus = await GetMenus(order.Items.Select(x => x.FoodId).ToList());
            var foods = await GetFoods(order.Items.Select(x => x.FoodId).ToList());

            var items = order.Items.Select(x =>
            {
                var item = menus[x.FoodId].Items.First(i => i.FoodId == x.FoodId);
                return new CartItemModel
                {
                    Id = x.Id,
                    Name = x.FoodName,
                    Price = x.UnitPrice,
                    Quantity = x.Units,
                    Image = item.ImageUrl,
                    RemainingQuantity = foods[x.FoodId].Quantity
                };
            });

            return new BaseResponse<CartModel>
            {
                Message = "cart",
                IsSuccessful = true,
                Value = new CartModel
                {
                    TotalPrice = order.Items.Select(x => x.UnitPrice * x.Units).Sum(),
                    Items = [.. items]
                }
            };
        }

        private async Task<Dictionary<int, Menu>> GetMenus(List<int> foodIds)
        {
            var menus = new Dictionary<int, Menu>();
            foreach (var id in foodIds)
            {
                var menu = await _menuRepository.GetAsync(m => m.Items.Any(i => i.FoodId == id));
                menus.Add(id, menu);
            }
            return menus;
        }

        private async Task<Dictionary<int, Food>> GetFoods(List<int> foodIds)
        {
            var foods = new Dictionary<int, Food>();
            foreach (var id in foodIds)
            {
                var food = await _foodRepository.GetAsync(f => f.Id == id);
                foods.Add(id, food);
            }
            return foods;
        }

        public async Task<int> LoggedinCustomerOrderCount()
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            var customer = await _customerRepository.GetAsync(x => x.User.Email == loginUserEmail);
            if (customer == null)
                return 0;
            var orders = await _orderRepository.GetAllAsync(x => x.CustomerId == customer.Id && x.OrderStatus != Status.Pending);
            return orders.Count;
        }

        public async Task<int> ChangeOrderItemQuantity(Guid id, int sign)
        {
            var orderItem = await _orderItemRepository.GetAsync(id);
            var food = await _foodRepository.GetAsync(orderItem.FoodId);
            if ((orderItem.Units == 1 && sign < 0) || food.Quantity == 0)
                return 0;
            orderItem.Units += sign;
            food.Quantity -= sign;
            _foodRepository.Update(food);
            _orderItemRepository.Update(orderItem);
            await _unitOfWork.SaveAsync();
            return orderItem.Units;
        }
    }
}
