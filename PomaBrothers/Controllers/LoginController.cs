using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;

namespace PomaBrothers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public readonly PomaBrothersDbContext _context;
        public LoginController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<Employee>> Login(string user, string password)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }

            var userEntity = await _context.Employees
                .FirstOrDefaultAsync(e => e.User == user && e.Password == password);

            if (userEntity == null)
            {
                return NotFound();
            }
            return Ok(userEntity);
        }
    }

    
}
