using Microsoft.AspNetCore.Mvc;
using DrinkShop.Application.Interfaces;
using DrinkShop.Domain.Entities;

namespace DrinkShop.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DrinkController : ControllerBase
    {
        private readonly IDrinkService _service;
        public DrinkController(IDrinkService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var drinks = await _service.GetAllAsync();
            return Ok(new { code = 200, message = "Success", data = drinks });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var drink = await _service.GetByIdAsync(id);
            if (drink == null)
                return NotFound(new { code = 404, message = "Not Found" });
            return Ok(new { code = 200, message = "Success", data = drink });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Drink drink)
        {
            await _service.AddAsync(drink);
            return Ok(new { code = 200, message = "Created", data = drink });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { code = 200, message = "Deleted" });
        }
    }
}
