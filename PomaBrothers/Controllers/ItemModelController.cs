using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;

namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemModelController : Controller
    {
        private readonly PomaBrothersDbContext _context;

        public ItemModelController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMany")]
        public async Task<IActionResult> GetMany()
        {
            List<ItemModel> models = await _context.Item_Model.ToListAsync();
            if (models.Count == 0)
            {
                return BadRequest();
            }
            return Ok(models);
        }

        [HttpGet]
        [Route("GetOne/{id:int}")]
        public async Task<ActionResult<ItemModel>> GetOne(int id)
        {
            var getModel = await _context.Item_Model.FirstOrDefaultAsync(x => x.Id == id);
            if (getModel != null)
            {
                return Ok(getModel);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> New([FromBody]ItemModel model)
        {
            if (model.CapacityOrSize == 0)
                model.CapacityOrSize = null;
            if (model != null)
            {
                try
                {
                    await _context.Item_Model.AddAsync(model!);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("New", "ItemModel", model);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromBody]ItemModel model)
        {
            var getModel = await FindById(model.Id);
            if (getModel != null)
            {
                try
                {
                    _context.Entry(getModel).CurrentValues.SetValues(model);
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
        public async Task<IActionResult> Remove([FromRoute] int id)
        {
            var getModel = await FindById(id);
            if (getModel != null)
            {
                try
                {
                    _context.Item_Model.Remove(getModel);
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

        [HttpGet]
        [Route("SearchModel/{likeModel}")]
        public async Task<ActionResult<List<ItemModel>>> SearchModel([FromRoute]string likeModel)
        {
            var results = await _context.Item_Model.Where(f => f.ModelName.Contains(likeModel) || f.Marker.Contains(likeModel))
                .Select(model => new ItemModel 
                {
                    ModelName = model.ModelName,
                    Marker = model.Marker,
                    Id = model.Id, 
                    CapacityOrSize = model.CapacityOrSize
                })
                .ToListAsync();
            return Ok(results);
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ItemModel> FindById(int id)
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
