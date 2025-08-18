using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;

namespace DrinkShop.Application.Services
{
    public class DrinkService : IDrinkService
    {
        private readonly IDrinkRepository _repo;
        public DrinkService(IDrinkRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Drink>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Drink?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task AddAsync(Drink drink) => _repo.AddAsync(drink);
        public Task UpdateAsync(Drink drink) => _repo.UpdateAsync(drink);
        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
        public Task<List<Drink>> GetDrinksByIdsAsync(List<int> ids) => _repo.GetDrinksByIdsAsync(ids);
        public async Task UpdateDrinksAsync(List<Drink> drinks)
        {
            foreach (var drink in drinks)
            {
                await _repo.UpdateAsync(drink);
            }
        }
    }
}
