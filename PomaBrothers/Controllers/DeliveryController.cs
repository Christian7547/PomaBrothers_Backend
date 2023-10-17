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
            var getInvoices = await JoinItemsWithInvoices();
            var query = getInvoices.Join(_context.Suppliers, gi => gi.SupplierId, s => s.Id,
                (gi, s) => new
                {
                    Invoice = gi,
                    Supplier = s
                }).GroupBy(q => q.Invoice.Id).ToList();
            List<Invoice> invoices = new();
            foreach (var group in query)
            {
                var invoice = group.First().Invoice;
                invoice.Supplier = group.Select(g => g.Supplier).First();
                invoices.Add(invoice);
            }
            return Ok(invoices.OrderByDescending(i => i.RegisterDate));
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<Invoice>> JoinItemsWithInvoices()
        {
            var getItems = await JoinItemsWithModels();
            var getInvoices = await GetInvoicesWithDetails();
            foreach (var invoice in getInvoices)
            {
                var query = invoice.DeliveryDetails!.Join(getItems, dd => dd.ItemId, i => i.Id,
                    (dd, i) => new DeliveryDetail
                    {
                        PurchasePrice = dd.PurchasePrice,
                        Item = i
                    }).ToList();
                invoice.DeliveryDetails = query;
            }
            return getInvoices;
        } 

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<Invoice>> GetInvoicesWithDetails()
        {
            List<Invoice> invoices = new();
            var query = await _context.Invoices.Join(_context.DeliveryDetails, i => i.Id, dd => dd.InvoiceId,
                (i, dd) => new 
                {
                    Invoice = i,
                    Details = dd
                }).GroupBy(q => q.Invoice.Id).ToListAsync();
            foreach(var group in query)
            {
                var invoice = group.First().Invoice;
                invoice.DeliveryDetails = group.Select(g => g.Details).ToList();
                invoices.Add(invoice);
            }
            return invoices;    
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<Item>> JoinItemsWithModels()
        {
            var items = new List<Item>();
            var query = await _context.Items.Join(_context.Item_Model, i => i.ModelId, im => im.Id,
                (i, im) => new 
                {
                    id = i.Id,
                    name = i.Name,
                    serie = i.Serie,
                    modelName = im.ModelName,
                    marker = im.Marker
                }).ToListAsync();
            foreach (var i in query)
                items.Add(new Item
                {
                    Id = i.id,
                    Name = i.name,
                    Serie = i.serie,
                    ItemModel = new ItemModel
                    {
                        ModelName = i.modelName,
                        Marker = i.marker
                    }
                });
            return items;
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
