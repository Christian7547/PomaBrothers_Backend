using Microsoft.AspNetCore.Http;
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
                return Ok(query);
            }
            return Ok("No logs");
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> New(Category category)
        {
            try
            {
                if(category != null)
                {
                    await _context.Categories.AddAsync(category);
                    var save = await _context.SaveChangesAsync();
                    return CreatedAtAction("New", "Category", category);
                }
                throw new Exception("Internal Server Error");
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
