using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrinkShop.Infrastructure.Repositories
{
    /// <summary>
    /// 商品資料存取實作
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly DrinkShopDbContext _db;
        public ProductRepository(DrinkShopDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _db.Products.OrderByDescending(p => p.Id).ToListAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Price,
                Category = p.Category ?? string.Empty,
                ImageUrl = p.ImageUrl ?? string.Empty,
                Stock = p.Stock,
                IsActive = p.IsActive
            });
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return null;
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Price,
                Category = p.Category ?? string.Empty,
                ImageUrl = p.ImageUrl ?? string.Empty,
                Stock = p.Stock,
                IsActive = p.IsActive
            };
        }

        public async Task<ProductDto> CreateAsync(ProductDto input)
        {
            var product = new Product
            {
                Name = input.Name,
                Description = input.Description,
                Price = input.Price,
                Category = input.Category,
                ImageUrl = input.ImageUrl,
                Stock = input.Stock,
                IsActive = input.IsActive
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            input.Id = product.Id;
            return input;
        }

        public async Task<ProductDto?> UpdateAsync(int id, ProductDto input)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return null;
            product.Name = input.Name;
            product.Description = input.Description;
            product.Price = input.Price;
            product.Category = input.Category;
            product.ImageUrl = input.ImageUrl;
            product.Stock = input.Stock;
            product.IsActive = input.IsActive;
            await _db.SaveChangesAsync();
            return input;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
