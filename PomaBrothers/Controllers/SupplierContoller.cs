
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;
using PomaBrothers.Models.DTOModels;
namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {

        private readonly PomaBrothersDbContext _context;

        public SupplierController(PomaBrothersDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetMany")]
        public async Task<IActionResult> GetMany()
        {
            List<Supplier> supplier = await _context.Suppliers.Where(i => i.Status.Equals(1)).ToListAsync();
            if (supplier.Count == 0)
            {
                return BadRequest();
            }
            return Ok(supplier);
        }

        [HttpGet]
        [Route("GetOne/{id:int}")]
        public async Task<ActionResult<Supplier>> GetOne([FromRoute] int id)
        {
            var getSupplier = await FindById(id);
            if (getSupplier != null)
            {
                return Ok(getSupplier);
            }
            return BadRequest();
        }


        [HttpPost]
        [Route("NewSupplier")]
        public async Task<IActionResult> NewSupplier([FromBody] Supplier supplier)
        {
            try
            {
                if (supplier != null)
                {
                    await _context.Suppliers.AddAsync(supplier);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("NewSupplier", "Supplier", supplier);
                }
                throw new Exception("Internal server error");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("EditSupplier")]
        public async Task<IActionResult> Edit([FromBody] Supplier supplier)
        {
            try
            {
                var Idfound = await FindById(supplier.Id);
                if (Idfound != null)
                {
                    supplier.RegisterDate = Idfound.RegisterDate; //The registerDate cannot be changed
                    _context.Entry(Idfound).CurrentValues.SetValues(supplier); //load the existing entity from the context ('found') using the same Id and then update the properties of that entity with the values of the 'item' object.
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
        [Route("RemoveSupplier/{id:int}")]
        public async Task<IActionResult> Remove([FromRoute] int id)
        {
            var Idfound = await FindById(id);
            if (Idfound != null)
            {
                try
                {
                    Idfound.Status = 0;
                    _context.Entry(Idfound).State = EntityState.Modified;
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

        [HttpGet, Route("SearchSupplier/{likeSupplier}")]
        public async Task<IActionResult> SearchSupplier([FromRoute]string likeSupplier)
        {
            var results = await _context.Suppliers.Where(s => s.BussinesName.Contains(likeSupplier))
                .Select(s => new SupplierDTO
                {
                    SupplierId = s.Id,
                    SupplierBussinesName = s.BussinesName
                }).ToListAsync();
            return Ok(results);
        }

        //busca un elemento (o registro) en la base de datos utilizando el Entity Framework Core
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)] //este método no se documentará en la interfaz de Swagger.
        public async Task<Supplier> FindById(int id)
        {
            var query = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == id);
            if (query != null)
            {
                return query;
            }
            return null!;
        }
    }
}
