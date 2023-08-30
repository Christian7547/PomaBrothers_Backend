using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;

namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public ItemController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMany")]
        public async Task<ActionResult<List<Item>>> GetMany()
        {
            var query = await _context.Items.ToListAsync();
            if (query != null)
            {
                return Ok(query);
            }
            return Ok("No logs");
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> New([FromBody]Item item)
        {
            try
            {
                if (item != null)
                {
                    await _context.Items.AddAsync(item);
                    var save = await _context.SaveChangesAsync();
                    return CreatedAtAction("New", "Item", item);
                }
                throw new Exception("Internal server error");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(Item item)
        {
            try
            {
                var found = await FindById(item.Id);
                if (found != null)
                {
                    found = item;
                    _context.Items.Update(found);
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var found = await FindById(id);
                if (found != null)
                {
                    _context.Items.Remove(found);
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                return NotFound();

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)] //Indicates that Swagger does not generate documentation for this method
        public async Task<Item> FindById(int id)
        {
            var query = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (query != null)
            {
                return query;
            }
            return null!;
        }
    }
}