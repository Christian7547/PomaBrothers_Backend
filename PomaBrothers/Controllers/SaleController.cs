using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;


namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public SaleController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMany")]
        public async Task<IActionResult> GetMany()
        {
            return Ok(await GetSales());
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<Sale>> GetSales()
        {
            var sales = await _context.Sales.Include(s => s.SaleDetails)!
                .ThenInclude(sd => sd.Item)
                .Include(s => s.Customer)
                .ToListAsync();
            return sales;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] Sale saleCreateModel)
        {
            if (saleCreateModel == null)
            {
                return BadRequest("Los datos de venta son nulos");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Aquí puedes realizar validaciones adicionales si es necesario

            try
            {
                // Crear una instancia de Sale a partir de SaleCreateModel
                var sale = new Sale
                {
                    EmployeeId = saleCreateModel.EmployeeId,
                    CustomerId = saleCreateModel.CustomerId,
                    Total = saleCreateModel.Total,
                    RegisterDate = DateTime.Now,
                    // Otros campos
                };

                // Agregar la venta al contexto de datos
                _context.Sales.Add(sale);

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok(sale);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la venta: {ex.Message}");
            }
        }
    }
}