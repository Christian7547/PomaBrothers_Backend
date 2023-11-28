using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;
using System.Security.Cryptography;
using System.Text;

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

            password = GetSHA256(password);
            var userEntity = await _context.Employees
                .FirstOrDefaultAsync(e => e.User == user && e.Password == password);

            if (userEntity == null)
            {
                return NotFound();
            }
            return Ok(userEntity);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private string GetSHA256(string str)
        {
            SHA256 sha256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null!;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}
