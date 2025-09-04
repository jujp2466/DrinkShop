using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Services
{
	/// <summary>
	/// 飲品服務層，僅透過 Repository 存取資料
	/// </summary>
	public class DrinkService : IDrinkService
	{
		private readonly IDrinkRepository _drinkRepository;
		public DrinkService(IDrinkRepository drinkRepository)
		{
			_drinkRepository = drinkRepository;
		}

		/// <summary>
		/// 取得所有飲品
		/// </summary>
		public async Task<IEnumerable<DrinkDto>> GetAllAsync()
		{
			return await _drinkRepository.GetAllDrinksAsync();
		}

		/// <summary>
		/// 依 ID 取得飲品
		/// </summary>
		public async Task<DrinkDto?> GetByIdAsync(int id)
		{
			return await _drinkRepository.GetDrinkByIdAsync(id);
		}
		// ...其他方法依需求擴充
	}
}
