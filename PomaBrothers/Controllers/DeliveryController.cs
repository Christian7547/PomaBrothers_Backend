using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;
using PomaBrothers.Models.DTOModels;

namespace PomaBrothers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly PomaBrothersDbContext _context;

        public DeliveryController(PomaBrothersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMany")]
        public async Task<IActionResult> GetMany()
        {
            List<Item> items = new();
            var query = await _context.Invoices.Join(_context.DeliveryDetails.Join(_context.Items, d => d.ItemId, i => i.Id,
                (d, i) => new
                {
                    Item = i,
                    InvoiceID = d.InvoiceId,
                    purchasePrice = d.PurchasePrice
                }), i => i.Id, d => d.InvoiceID, 
                (i, d) => new
                {
                    item = d.Item,
                    price = d.purchasePrice,
                    Register = i.RegisterDate,
                    TotalInvoice = i.Total,
                    list = items
                }).ToListAsync();
            foreach (var i in query)
                items.Add(i.item);
            return Ok(query);
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> New([FromBody] DeliveryDTO deliveryDTO)
        {
            List<DeliveryDetail> details = new();
            List<int> modelsId = new();
            Invoice invoice = new()
            {
                SupplierId = deliveryDTO.SupplierId,
                Total = Convert.ToDecimal(deliveryDTO.Total),
            };
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.Items.AddRangeAsync(deliveryDTO.Items);
                await _context.SaveChangesAsync();
                for (int i = 0; i < deliveryDTO.PurchasePrices.Count; i++)
                {
                    modelsId.Add(deliveryDTO.Items[i].ModelId);
                    details.Add(new DeliveryDetail()
                    {
                        ItemId = deliveryDTO.Items[i].Id,
                        PurchasePrice = deliveryDTO.PurchasePrices[i]
                    });
                }
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();
                foreach (var detail in details)
                    detail.InvoiceId = invoice.Id;
                await _context.DeliveryDetails.AddRangeAsync(details);
                await _context.SaveChangesAsync();
                await InsertManyInWarehouse(deliveryDTO.WarehouseId, modelsId);
                await transaction.CommitAsync();
                return CreatedAtAction("New", "Delivery", deliveryDTO);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task InsertManyInWarehouse(int warehouseId, List<int> models)
        {
            var getSections = _context.Sections.Where(s => s.WarehouseId.Equals(warehouseId));
            foreach(int modelId in models) 
            {
                var section = await getSections.FirstOrDefaultAsync(s => s.ModelId.Equals(modelId));
                if(section != null)
                {
                    section.ModelQuantity++;
                    _context.Entry(section).State = EntityState.Modified;
                }
                else
                {
                    await _context.Sections.AddAsync(new Section()
                    {
                        ModelId = modelId,
                        ModelQuantity = 1,
                        WarehouseId = warehouseId
                    });
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
