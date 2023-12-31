﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;

namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public ItemController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMany")]
        public async Task<IActionResult> GetMany()
        {
            List<Item> items = await _context.Items.ToListAsync();
            if(items.Count == 0)
            {
                return BadRequest();
            }
            return Ok(items);
        }

        [HttpGet]
        [Route("GetOne/{id:int}")]
        public async Task<ActionResult<Item>> GetOne(int id)
        {
            var getItem = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (getItem != null)
            {
                return Ok(getItem);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("FilterByCategory/{id:int}")]
        public async Task<ActionResult<List<Item>>> FilterByCategory(int id)
        {
            var sendItems = await _context.Items.Where(s => s.CategoryId.Equals(id)).ToListAsync();
            if(sendItems != null)
            {
                return Ok(sendItems);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> New([FromBody]Item item)
        {
            if (item != null)
            {
                try
                {
                    await _context.Items.AddAsync(item);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("New", "Item", item);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromBody] Item item)
        {
            var getItem = await FindById(item.Id);
            if (getItem != null)
            {
                try
                {
                    item.RegisterDate = getItem.RegisterDate; //The registerDate cannot be changed
                    _context.Entry(getItem).CurrentValues.SetValues(item); //load the existing entity from the context ('found') using the same Id and then update the properties of that entity with the values of the 'item' object.
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

        [HttpDelete]
        [Route("Remove/{id:int}")]
        public async Task<IActionResult> Remove([FromRoute]int id)
        {
            var getItem = await FindById(id);
            var details = _context.DeliveryDetails.Where(d => d.ItemId == getItem.Id);
            if (details.Count() > 0)
            {
                _context.DeliveryDetails.Remove(details.First());
                _context.Items.Remove(getItem);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                _context.Items.Remove(getItem!);
                await _context.SaveChangesAsync();
                return NoContent();
            }

        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)] //Indicates that Swagger does not generate documentation for this method
        public async Task<Item> FindById(int id)
        {
            var find = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (find != null)
            {
                return find;
            }
            return null!;
        }
    }
}