using DrinkShop.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductDto input);
        Task<ProductDto?> UpdateAsync(int id, ProductDto input);
        Task<bool> DeleteAsync(int id);
    }
}
