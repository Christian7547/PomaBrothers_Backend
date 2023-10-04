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
        public async Task<ActionResult<Employee>> GetEmployee(string username, string password)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            var user = await _context.Employees.ToListAsync();

            Employee User = null;

            foreach (var u in user)
            {
                if (u.User.Equals(username) && u.Password.Equals(password))
                {
                    User = u;
                }
            }

            if (User == null)
            {
                return NotFound();
            }

            return User;
        }
    }

    
}
