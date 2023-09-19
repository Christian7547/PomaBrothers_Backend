using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;

namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public SectionController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> New([FromBody]Section section)
        {
            if (section != null)
            {
                try
                {
                    await _context.Sections.AddAsync(section);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("New", "Section", section);
                }
                catch(Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("Remove/{id:int}")]
        public async Task<IActionResult> Remove([FromRoute]int id)
        {
            var section = await FindById(id);
            if (section != null)
            {
                try
                {
                    _context.Sections.Remove(section); 
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch(Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return NotFound();
        }

        [HttpGet]
        [Route("GetQuantityModel/{id:int}")]
        public async Task<ActionResult<int>> GetQuantityModel([FromRoute] int id) //id of Model
        {
            var query = await _context.Sections.Where(s => s.ModelId.Equals(id)).Select(s => s.ModelQuantity).FirstAsync();
            return Ok(query);
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Section> FindById(int id)
        { 
            var find = await _context.Sections.FirstOrDefaultAsync(x => x.Id == id);
            if (find != null)
            {
                return find;
            }
            return null!;
        }
    }
}
