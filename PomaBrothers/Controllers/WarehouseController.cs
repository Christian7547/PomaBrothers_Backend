using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;

namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public WarehouseController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMany")]
        public async Task<ActionResult<List<Warehouse>>> GetMany()
        {
            List<Warehouse> warehouses = await _context.Warehouses.ToListAsync();
            if(warehouses.Count > 0)
            {
                return Ok(warehouses);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("GetOne/{id:int}")]
        public async Task<ActionResult<Warehouse>> GetOne(int id)
        {
            var warehouse = await FindById(id);
            if(warehouse != null)
            {
                return Ok(warehouse);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> New([FromBody]Warehouse warehouse)
        {
            if(warehouse != null)
            {
                try
                {
                    await _context.Warehouses.AddAsync(warehouse);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("New", "Warehouse", warehouse);
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
            var warehouse = await FindById(id);
            if(warehouse != null)
            {
                try
                {
                    _context.Warehouses.Remove(warehouse);
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
        [Route("GetContentWarehouse/{id:int}")]
        public async Task<ActionResult<List<ItemModel>>> GetContentWarehouse([FromRoute]int id) //id of Warehouse
        {
            List<ItemModel> models = new();
            var query = await _context.Sections.Join(_context.Item_Model, s => s.ModelId, im => im.Id,
                (s, im) => new
                {
                    WarehouseID = s.WarehouseId,
                    Model = im,
                    Quantity = s.ModelQuantity
                }).Where(q => q.WarehouseID.Equals(id)).ToListAsync();
            if(query.Count > 0)
            {
                foreach (var item in query)
                    models.Add(item.Model);
                return Ok(models);
            }
            return null!;
        }

        [HttpGet]
        [Route("GetItemsWarehouse/{id:int}")]
        public async Task<ActionResult<List<Item>>> GetItemsWarehouse([FromRoute]int id)//id of Model
        {
            var query = await _context.Items.Where(i => i.ModelId.Equals(id)).ToListAsync();
            if(query.Count > 0)
            {
                return Ok(query);
            }
            return null!;
        }


        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Warehouse> FindById(int id)
        {
            var find = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
            if (find != null)
            {
                return find;
            }
            return null!;
        }
    }
}
