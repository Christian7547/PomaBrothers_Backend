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

        [HttpGet]
        [Route("GetOne/{id:int}")]
        public async Task<ActionResult<Item>> GetOne([FromRoute]int id)
        {
            var getItem = await FindById(id);
            if (getItem != null)
            {
                return Ok(getItem);
            }
            return BadRequest();
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
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit([FromBody]Item item)
        {
            try
            {
                var found = await FindById(item.Id);
                if (found != null)
                {
                    item.RegisterDate = found.RegisterDate; //The registerDate cannot be changed
                    _context.Entry(found).CurrentValues.SetValues(item); //load the existing entity from the context ('found') using the same Id and then update the properties of that entity with the values of the 'item' object.
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
        [Route("Remove/{id:int}")]
        public async Task<IActionResult> Remove([FromRoute]int id)
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