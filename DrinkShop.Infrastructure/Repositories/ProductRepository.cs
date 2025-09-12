using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrinkShop.Infrastructure.Repositories
{
    /// <summary>
    /// 商品資料存取實作
    /// </summary>
    // 此 Repository 負責與資料庫的 Product 表互動，並回傳/接收 DTO（ProductDto）。
    // 注意：CreatedAt 由資料庫預設值填入（例如在資料表定義中設定 DEFAULT current_timestamp），
    //       所以在 Create/Update 後需從實體讀回該欄位並填入 DTO，讓 API 能回傳給前端。
    public class ProductRepository : IProductRepository
    {
        private readonly DrinkShopDbContext _db;
        public ProductRepository(DrinkShopDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            // 取得所有商品，按 Id 倒序（最新在前）
            var products = await _db.Products.OrderByDescending(p => p.Id).ToListAsync();
            // 將 Entity 映射成 DTO，確保 null 字串欄位轉為空字串，並把 CreatedAt 傳回給客戶端
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Price,
                Category = p.Category ?? string.Empty,
                ImageUrl = p.ImageUrl ?? string.Empty,
                CreatedAt = p.CreatedAt,
                Stock = p.Stock,
                IsActive = p.IsActive
            });
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return null;
            // 單筆查詢：同樣將實體轉為 DTO，保留 CreatedAt
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Price,
                Category = p.Category ?? string.Empty,
                ImageUrl = p.ImageUrl ?? string.Empty,
                CreatedAt = p.CreatedAt,
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

            // 由於 CreatedAt 通常由資料庫預設（例如 current_timestamp）填入，
            // 在 SaveChanges 之後實體的 CreatedAt 欄位會有值，
            // 因此把該值回填到 DTO，讓 API 回傳給前端看到建立時間。
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description ?? string.Empty,
                Price = product.Price,
                Category = product.Category ?? string.Empty,
                ImageUrl = product.ImageUrl ?? string.Empty,
                CreatedAt = product.CreatedAt,
                Stock = product.Stock,
                IsActive = product.IsActive
            };
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
            // 更新後，確保回傳 DTO 包含 CreatedAt（資料庫原始建立時間）
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description ?? string.Empty,
                Price = product.Price,
                Category = product.Category ?? string.Empty,
                ImageUrl = product.ImageUrl ?? string.Empty,
                CreatedAt = product.CreatedAt,
                Stock = product.Stock,
                IsActive = product.IsActive
            };
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
