using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrinkShop.Domain.Entities;
using DrinkShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly DrinkShopDbContext _db;
        public ProductsController(DrinkShopDbContext db)
        {
            _db = db;
        }

        // GET /api/v1/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _db.Products
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return Ok(new { code = 200, message = "Success", data = products });
        }

        // GET /api/v1/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Success", data = product });
        }

        // POST /api/v1/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product input)
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
            return Ok(new { code = 200, message = "Created", data = product });
        }

        // PUT /api/v1/products/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product input)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { code = 404, message = "Not Found" });

            product.Name = input.Name;
            product.Description = input.Description;
            product.Price = input.Price;
            product.Category = input.Category;
            product.ImageUrl = input.ImageUrl;
            product.Stock = input.Stock;
            product.IsActive = input.IsActive;

            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Updated", data = product });
        }

        // DELETE /api/v1/products/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound(new { code = 404, message = "Not Found" });
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
