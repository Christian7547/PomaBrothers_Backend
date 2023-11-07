using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models.DTOModels;

namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public CustomerController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet, Route("SearchCustomer/{likeCustomer}")]
        public async Task<ActionResult<List<CustomerDTO>>> SearchCustomer([FromRoute] string likeCustomer)
        {
            var results = await _context.Customers.Where(c => c.Ci.Contains(likeCustomer))
                .Select(customer => new CustomerDTO
                {
                    NameCustomer = customer.Name,
                    LastNameCustomer = customer.LastName,
                    SecondLastNameCustomer = customer.SecondLastName!,
                    Id = customer.Id
                })
                .ToListAsync();
            return Ok(results);
        }
    }
}
