using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrinkShop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// 取得所有商品
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        /// <summary>
        /// 依商品編號取得商品
        /// </summary>
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        /// <summary>
        /// 新增商品
        /// </summary>
        public async Task<ProductDto> CreateAsync(ProductDto input)
        {
            return await _repo.CreateAsync(input);
        }

        /// <summary>
        /// 更新商品
        /// </summary>
        public async Task<ProductDto?> UpdateAsync(int id, ProductDto input)
        {
            return await _repo.UpdateAsync(id, input);
        }

    /// <summary>
    /// 刪除商品
    /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}
