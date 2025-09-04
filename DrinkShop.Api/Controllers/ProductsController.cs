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


            /// <summary>
            /// 取得所有商品
            /// </summary>
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var products = await _service.GetAllAsync();
                return Ok(new { code = 200, message = "Success", data = products });
            }


        /// <summary>
        /// 依商品編號取得商品
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Success", data = product });
        }


        /// <summary>
        /// 新增商品
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto input)
        {
            var product = await _service.CreateAsync(input);
            return Ok(new { code = 200, message = "Created", data = product });
        }


        /// <summary>
        /// 更新商品
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto input)
        {
            var product = await _service.UpdateAsync(id, input);
            if (product == null) return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Updated", data = product });
        }


        /// <summary>
        /// 刪除商品
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
