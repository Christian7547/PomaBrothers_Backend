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
    }
}
