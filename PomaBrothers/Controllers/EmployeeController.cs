using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;

namespace PomaBrothers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public EmployeeController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetMany()
        {
            var query = await _context.Employees.ToListAsync();
            return Ok(query);
        } 
    }
}
