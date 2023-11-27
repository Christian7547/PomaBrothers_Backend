using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;
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


        [HttpGet]
        [Route("GetMany")]
        public async Task<ActionResult<List<Customer>>> GetMany()
        {
            var query = await _context.Customers.ToListAsync();
            if (query != null)
            {
                return Ok(query);
            }
            return Ok("No logs");
        }

        [HttpPost]
        [Route("CustomerCreate")]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            try
            {
                if (customer != null)
                {
                    customer.RegisterDate = DateTime.Now;
                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("Create", "Customer", customer);
                }
                throw new Exception("Internal server error");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
