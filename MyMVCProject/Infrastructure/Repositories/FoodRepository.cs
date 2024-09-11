using Microsoft.EntityFrameworkCore;
using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyMVCProject.Infrastructure.Repositories
{
    public class FoodRepository : IFoodRepository
    {
        private readonly RestaurantContext _context;
        public FoodRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<Food> AddAsync(Food food)
        {
            await _context.Foods.AddAsync(food);
            return food;
        }

        public async Task<bool> ExistsAsync(string name)
        {
            var exists = await _context.Foods.AnyAsync(x => x.Name == name);
            return exists;
        }

        public async Task<bool> ExistsAsync(string name, int id)
        {
            var exists = await _context.Foods.AnyAsync(x => x.Name == name && x.Id != id);
            return exists;
        }

        public async Task<ICollection<Food>> GetAllAsync()
        {
            var foods = await _context.Foods.ToListAsync();
            return foods;
        }

        public async Task<Food> GetAsync(int id)
        {
            var food = await _context.Foods.FirstOrDefaultAsync(x => x.Id == id);
            return food;
        }

        public async Task<Food> GetAsync(Expression<Func<Food, bool>> exp)
        {
            var food = await _context.Foods.FirstOrDefaultAsync(exp);
            return food;
        }

        public void Remove(Food food)
        {
            _context.Foods.Remove(food);
        }

        public Food Update(Food food)
        {
            _context.Foods.Update(food);
            return food;
        }
    }
}
