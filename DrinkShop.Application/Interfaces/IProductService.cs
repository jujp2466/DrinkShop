using DrinkShop.Application.DTOs;

namespace DrinkShop.Application.Interfaces
{
    public interface IProductService
    {
    /// <summary>
    /// 取得所有商品
    /// </summary>
    Task<IEnumerable<ProductDto>> GetAllAsync();
    /// <summary>
    /// 依商品編號取得商品
    /// </summary>
    Task<ProductDto?> GetByIdAsync(int id);
    /// <summary>
    /// 新增商品
    /// </summary>
    Task<ProductDto> CreateAsync(ProductDto input);
    /// <summary>
    /// 更新商品
    /// </summary>
    Task<ProductDto?> UpdateAsync(int id, ProductDto input);
    /// <summary>
    /// 刪除商品
    /// </summary>
    Task<bool> DeleteAsync(int id);
    }
}
