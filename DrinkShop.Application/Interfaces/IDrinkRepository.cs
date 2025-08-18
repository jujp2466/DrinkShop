using DrinkShop.Domain.Entities;

namespace DrinkShop.Application.Interfaces
{
    public interface IDrinkRepository
    {
        Task<IEnumerable<Drink>> GetAllAsync();
        Task<Drink?> GetByIdAsync(int id);
        Task AddAsync(Drink drink);
        Task UpdateAsync(Drink drink);
        Task DeleteAsync(int id);
        Task<List<Drink>> GetDrinksByIdsAsync(List<int> ids);
    }
}
