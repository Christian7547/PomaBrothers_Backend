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
        public async Task<IActionResult> GetMany()
        {
            List<Item> items = await _context.Items.ToListAsync();
            if (items.Count == 0)
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
                var getModel = await GetModel(getItem.ModelId);
                getItem.ItemModel = getModel;
                return Ok(getItem);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("FilterByCategory/{id:int}")]
        public async Task<ActionResult<List<Item>>> FilterByCategory(int id)
        {
            var sendItems = await _context.Items.Where(s => s.CategoryId.Equals(id)).ToListAsync();
            if (sendItems != null)
            {
                return Ok(sendItems);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("GetModels")]
        public async Task<IActionResult> GetModels()
        {
            List<ItemModel> models = await _context.Item_Model.ToListAsync();
            if (models.Count == 0)
            {
                return BadRequest();
            }
            return Ok(models);
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> New([FromBody] Item item)
        {
            if (item != null && item.ItemModel != null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        await _context.Item_Model.AddAsync(item.ItemModel);
                        await _context.SaveChangesAsync();

                        item.ModelId = item.ItemModel.Id;

                        await _context.Items.AddAsync(item);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return CreatedAtAction("New", "Item", item);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                    }
                }
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromBody] Item item)
        {
            var getItem = await FindById(item.Id);
            var getModel = await GetModel(item.ItemModel!.Id);
            if (getItem != null && getModel != null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        item.RegisterDate = getItem.RegisterDate; //The registerDate cannot be changed

                        _context.Entry(getModel).CurrentValues.SetValues(item.ItemModel);
                        await _context.SaveChangesAsync();

                        _context.Entry(getItem).CurrentValues.SetValues(item); //load the existing entity from the context ('found') using the same Id and then update the properties of that entity with the values of the 'item' object.
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return NoContent();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                    }
                }
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("Remove/{id:int}")]
        public async Task<IActionResult> Remove([FromRoute] int id)
        {
            var getItem = await FindById(id);
            if (getItem != null)
            {
                var getModel = await GetModel(getItem.ModelId);
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Items.Remove(getItem);
                        _context.Item_Model.Remove(getModel);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return NoContent();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                    }
                }
            }
            return NotFound();
        }

        //busca un elemento (o registro) en la base de datos utilizando el Entity Framework Core
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

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ItemModel> GetModel(int id)
        {
            var find = await _context.Item_Model.FirstOrDefaultAsync(x => x.Id == id);
            if (find != null)
            {
                return find;
            }
            return null!;
        }
    }
}