using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        [Route("GetMany")]
        public async Task<ActionResult<List<Employee>>> GetMany()
        {
            var employees = await _context.Employees.ToListAsync();
            if (employees != null)
            {
                return Ok(employees);
            }
            return Ok("No employees found");
        }

        [HttpGet]
        [Route("GetOne/{id:int}")]
        public async Task<ActionResult<Employee>> GetOne([FromRoute] int id)
        {
            var employee = await FindById(id);
            if (employee != null)
            {
                return Ok(employee);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("NewEmployee")]
        public async Task<IActionResult> NewEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee != null)
                {
                    employee.Password = GetSHA256(employee.Password);
                    employee.RegisterDate = DateTime.Now;
                    await _context.Employees.AddAsync(employee);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("NewEmployee", "Employee", employee);
                }
                throw new Exception("Internal server error");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("EditEmployee")]
        public async Task<IActionResult> EditEmployee([FromBody] Employee employee)
        {
            try
            {
                var employeeFound = await FindById(employee.Id);
                if (employeeFound != null)
                {
                    // The RegisterDate cannot be changed
                    employee.RegisterDate = employeeFound.RegisterDate;

                    // Update the properties of the existing entity with the values of the 'employee' object
                    _context.Entry(employeeFound).CurrentValues.SetValues(employee);

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
        [Route("RemoveEmployee/{id:int}")]
        public async Task<IActionResult> RemoveEmployee([FromRoute] int id)
        {
            try
            {
                var employeeFound = await FindById(id);
                if (employeeFound != null)
                {
                    _context.Employees.Remove(employeeFound);
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

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Employee> FindById(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            return employee;
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
