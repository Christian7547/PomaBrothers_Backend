using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;
using PomaBrothers.Models.DTOModels;

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
        public async Task<IActionResult> GetMany()
        {
            List<Item> items = await _context.Items.Where(i => i.Status.Equals(1)).ToListAsync();
            if(items.Count == 0)
            {
                return BadRequest();
            }
            return Ok(items);
        }

        [HttpGet]
        [Route("GetOne/{id:int}")]
        public async Task<ActionResult<Item>> GetOne(int id)
        {
            var getItem = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (getItem != null)
            {
                return Ok(getItem);
            }
            return NotFound();
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromBody] Item item)
        {
            var getItem = await FindById(item.Id);
            if (getItem != null)
            {
                try
                {
                    item.RegisterDate = getItem.RegisterDate; //The registerDate cannot be changed
                    _context.Entry(getItem).CurrentValues.SetValues(item); //load the existing entity from the context ('found') using the same Id and then update the properties of that entity with the values of the 'item' object.
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("Remove/{id:int}")]
        public async Task<IActionResult> Remove([FromRoute]int id)
        {
            var getItem = await FindById(id);
            if(getItem != null)
            {
                try
                {
                    getItem.Status = 0;
                    _context.Entry(getItem).State = EntityState.Modified;
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

        [HttpGet, Route("SearchProduct/{likeProduct}")]
        public async Task<ActionResult<List<ProductSearchDTO>>> SearchModel([FromRoute]string likeProduct)
        {
            var results = await _context.Items.Where(i => i.Name.Contains(likeProduct) || i.Serie.Contains(likeProduct))
                .Select(item => new ProductSearchDTO
                {
                    ProductId = item.Id,
                    ProductName = item.Name,
                    ProductSerie = item.Serie
                })
                .ToListAsync();
            return Ok(results);
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)] //Indicates that Swagger does not generate documentation for this method
        public async Task<Item> FindById(int id)
        {
            var find = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (find != null)
            {
                return find;
            }
            return null!;
        }
    }
}