﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> GetMany()
        {
            List<Employee> employees = await _context.Employees.Where(i => i.Status.Equals(1)).ToListAsync();
            if (employees.Count == 0)
            {
                return BadRequest();
            }
            return Ok(employees);
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
            var getEmployee = await FindById(id);
            if (getEmployee != null)
            {
                try
                {
                    getEmployee.Status = 0;
                    _context.Entry(getEmployee).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return NotFound();
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Employee> FindById(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            return employee;
        }
    }
}
