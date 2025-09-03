using DrinkShop.Application.DTOs;
using DrinkShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service)
        {
            _service = service;
        }

        // GET /api/v1/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(new { code = 200, message = "Success", data = products });
        }

        // GET /api/v1/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Success", data = product });
        }

        // POST /api/v1/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto input)
        {
            var product = await _service.CreateAsync(input);
            return Ok(new { code = 200, message = "Created", data = product });
        }

        // PUT /api/v1/products/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto input)
        {
            var product = await _service.UpdateAsync(id, input);
            if (product == null) return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Updated", data = product });
        }

        // DELETE /api/v1/products/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
