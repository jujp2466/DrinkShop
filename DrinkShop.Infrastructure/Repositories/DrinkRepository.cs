using DrinkShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DrinkShop.Application.Interfaces;

namespace DrinkShop.Infrastructure.Repositories
{
    public class DrinkRepository : IDrinkRepository
    {
        private readonly DrinkShopDbContext _context;
        public DrinkRepository(DrinkShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Drink>> GetAllAsync()
        {
            return await _context.Drinks.ToListAsync();
        }

        public async Task<Drink?> GetByIdAsync(int id)
        {
            return await _context.Drinks.FindAsync(id);
        }

        public async Task AddAsync(Drink drink)
        {
            _context.Drinks.Add(drink);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Drink drink)
        {
            _context.Drinks.Update(drink);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var drink = await _context.Drinks.FindAsync(id);
            if (drink != null)
            {
                _context.Drinks.Remove(drink);
                await _context.SaveChangesAsync();
            }
        }
    }
}
