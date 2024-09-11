using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Domain.Entities;

namespace MyMVCProject.Infrastructure.Context
{
    public class RestaurantContext : DbContext
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options) 
        {
            
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Food> Foods => Set<Food>();
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Wallet> Wallets => Set<Wallet>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Customer>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Food>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Menu>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Order>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Role>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().Property<int>("Id").ValueGeneratedOnAdd();
            modelBuilder.Entity<Wallet>().Property<int>("Id").ValueGeneratedOnAdd();



            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, DateCreated = DateTime.Now, Name = "Admin", CreatedBy = "1" },
                new Role { Id = 2, DateCreated = DateTime.Now, Name = "Chef", CreatedBy = "1" },
                new Role { Id = 3, DateCreated = DateTime.Now, Name = "Customer", CreatedBy = "1" } );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    DateCreated = DateTime.Now,
                    Email = "admin@gmail.com",
                    Password = "admin",
                    FirstName = "Olaniyi",
                    LastName = "Wadood",
                    DateOfBirth = new DateTime(2000, 3, 19),
                    Gender = Core.Domain.Enums.Gender.Male,
                    PhoneNumber = "09137191028",
                    RoleId = 1,
                    CreatedBy = "1",
                    ImageUrl = "C:\\Users\\WADOOD\\OneDrive\\Pictures\\Pictures\\profile1.png"
                });

            modelBuilder.Entity<Wallet>().HasData(
                new Wallet
                {
                    Id = 1,
                    DateCreated = DateTime.Now,
                    UserId = 1,
                    CreatedBy = "1"
                });
        }
    }
}
