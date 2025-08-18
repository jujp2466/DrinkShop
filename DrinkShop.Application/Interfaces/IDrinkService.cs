using DrinkShop.Domain.Entities;

namespace DrinkShop.Application.Interfaces
{
    public interface IDrinkService
    {
        Task<IEnumerable<Drink>> GetAllAsync();
        Task<Drink?> GetByIdAsync(int id);
        Task AddAsync(Drink drink);
        Task UpdateAsync(Drink drink);
        Task DeleteAsync(int id);
        Task<List<Drink>> GetDrinksByIdsAsync(List<int> ids);
        Task UpdateDrinksAsync(List<Drink> drinks);
    }
}
