using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Infrastructure.Context;
using MyMVCProject.Infrastructure.Repositories;

namespace MyMVCProject.Extensions
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddContext(this IServiceCollection services, string connectionString)
        {
            return services
                .AddDbContext<RestaurantContext>(a => a.UseMySQL(connectionString));
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IFileRepository, FileRepository>()
                .AddScoped<IFoodRepository, FoodRepository>()
                .AddScoped<IMenuRepository, MenuRepository>()
                .AddScoped<IMenuItemRepository, MenuItemRepository>()
                .AddScoped<IOrderRepository, OrderRepository>()
                .AddScoped<IOrderItemRepository, OrderItemRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IWalletRepository, WalletRepository>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<IFoodService, FoodService>()
                .AddScoped<IMenuService, MenuService>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IWalletService, WalletService>()
                .AddScoped<NavigationHistoryManager>();
        }
    }
}
