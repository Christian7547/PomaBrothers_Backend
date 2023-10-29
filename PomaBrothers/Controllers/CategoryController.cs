using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;

namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public CategoryController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMany")]
        public async Task<ActionResult<List<Category>>> GetMany()
        {
            var query = await _context.Categories.ToListAsync();
            if (query != null)
            {
                return Ok(query.OrderBy(c => c.Name));
            }
            return NotFound();
        }

        [HttpGet]
        [Route("FilterByCategory/{id:int}")]
        public async Task<IActionResult> FilterByCategory(int id)
        {
            var sendItems = await _context.Items.Include(i => i.ItemModel)
                .Where(i => i.CategoryId == id && i.Status == 1)
                .ToListAsync();   
            if(sendItems.Count > 0)
            {
                return Ok(sendItems);
            }
            return NotFound("No hay registros para esta categoría");
        }
    }
}