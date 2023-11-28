using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PomaBrothers.Data;
using PomaBrothers.Models;
using PomaBrothers.Models.DTOModels;
using static System.Collections.Specialized.BitVector32;


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
        public async Task<List<Sale>> GetMany()
        {
            var sales = await _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Item)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.RegisterDate) // Ordena por fecha de registro descendente
                .ToListAsync();
            return sales;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<Sale>> GetSales()
        {
            var sales = await _context.Sales
                .Include(s => s.SaleDetails)!
                .ThenInclude(sd => sd.Item)
                .Include(s => s.Customer)
                .ToListAsync();
            return sales;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] SaleDetailDTO saleDetailDTO)
        {
            if (saleDetailDTO == null || saleDetailDTO.ProductSaled == null || !saleDetailDTO.ProductSaled.Any())
            {
                return BadRequest("La venta y sus detalles no pueden ser nulos y deben contener al menos un detalle.");
            }

            List<SaleDetail> saleDetails = new List<SaleDetail>();

            Sale sale = new Sale()
            {
                CustomerId = saleDetailDTO.CustomerId,
                EmployeeId = saleDetailDTO.EmployeeId,
                Total = saleDetailDTO.Total,
                RegisterDate = saleDetailDTO.RegisterDate,
            };
            await using var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.Sales.AddAsync(sale);
                await _context.SaveChangesAsync();
                foreach (var item  in saleDetailDTO.ProductSaled)
                {
                    SaleDetail product = new SaleDetail();
                    product.IdItem = item.ProductId;
                    product.Subtotal = item.ProductPrice;
                    product.IdSale = sale.Id;
                    saleDetails.Add(product);
                    await ChangeStatusItem(item.ProductId, item.Status);
                    await DiscountWarehouse(item.ModelId);
                }
                await _context.SaleDetails.AddRangeAsync(saleDetails);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(saleDetails);
            }catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task ChangeStatusItem(int productId, byte newStatus)
        {
            var product = await _context.Items.FindAsync(productId);
            if (product != null)
            {
                product.Status = newStatus;
                _context.Entry(product).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task DiscountWarehouse(int model)
        {
            var getSections = await _context.Sections.Where(s => s.ModelId.Equals(model)).FirstOrDefaultAsync();
            if (getSections != null)
            {
                if(getSections.ModelQuantity > 0)
                    getSections.ModelQuantity--;
                _context.Entry(getSections).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSale(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }
            return Ok(sale);
        }

        [HttpGet]
        [Route("GetLatest")]
        public async Task<Sale> GetLatest()
        {
            var latestSale = await _context.Sales
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Item)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.Id) // Ordena por ID de forma descendente
                .FirstOrDefaultAsync(); // Selecciona la venta con el ID más alto
            return latestSale;
        }
    }
}