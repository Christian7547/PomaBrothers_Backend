using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;
using PomaBrothers.Models.DTOModels;
using PomaBrothers.Reports.Interfaces;

namespace PomaBrothers.Reports.Implementation               
{
    public class DeliveryReportsService : IDeliveryReportsService
    {
        private readonly PomaBrothersDbContext _context;
        private ProductSupplierDTO? _productSupplierDTO;

        public DeliveryReportsService(PomaBrothersDbContext context) => _context = context;

        #region OrdersDateRange
        public async Task<List<DeliveryDetail>> OrdersByDateRangeReport(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.AddDays(1);
            var getItems = await _context.Items
                .Where(i => i.RegisterDate >= startDate && i.RegisterDate <= endDate && i.Status != 0)
                .Select(i => new Item
                {
                    Id = i.Id,
                    Name = i.Name,
                    Serie = i.Serie,
                    RegisterDate = i.RegisterDate,
                    ModelId = i.ModelId
                })
                .ToListAsync();
            var modelsWithItems = GetModels(getItems);
            return GetItemsWithPurchasePrices(modelsWithItems);
        }

        public List<DeliveryDetail> GetItemsWithPurchasePrices(List<Item> items)
        {
            var list = new List<DeliveryDetail>();
            var query = items.Join(_context.DeliveryDetails, i => i.Id, dd => dd.ItemId, 
                (i, dd) => new DeliveryDetail
                {
                    PurchasePrice = dd.PurchasePrice,
                    Item = i
                }).ToList();
            return query!;
        }

        public List<Item> GetModels(List<Item> items)
        {
            var models = items.Join(_context.Item_Model, i => i.ModelId, im => im.Id,
                (i, im) => new Item
                {
                    Id = i.Id,
                    Name = i.Name,
                    Serie = i.Serie,
                    RegisterDate = i.RegisterDate,
                    ItemModel = new ItemModel
                    {
                        ModelName = im.ModelName,
                        Marker = im.Marker,
                    }
                }).ToList();
            return models;
        }
        #endregion

        #region ItemWithSupplier
        public async Task<DeliveryDetail> GetDetails(int productId)
        {
            var getDetail = await _context.DeliveryDetails.Where(dd => dd.ItemId == productId)
                .Select(detail => new DeliveryDetail
                {
                    InvoiceId = detail.InvoiceId,
                    PurchasePrice = detail.PurchasePrice
                }).FirstOrDefaultAsync();
            return getDetail!;
        }

        public async Task<Supplier> GetSupplierAsync(int invoiceId)
        {
            var getInvoice = await _context.Suppliers.Join(_context.Invoices, s => s.Id, i => i.SupplierId, 
                (s, i) => new
                {
                    Supplier = new Supplier { BussinesName = s.BussinesName, Phone = s.Phone, Ci = s.Ci, Manager = s.Manager, Address = s.Address},
                    InvoiceID = i.Id
                }).Where(s => s.InvoiceID.Equals(invoiceId)).FirstOrDefaultAsync();
            var supplier = getInvoice?.Supplier;    
            return supplier!;
        }

        public async Task<Item> GetItemAsync(int productId)
        {
            var getItem = await _context.Items.Where(i => i.Id.Equals(productId))
                .Select(item => new Item
                {
                    Name = item.Name,
                    Serie = item.Serie,
                    Description = item.Description,
                    DurationWarranty = item.DurationWarranty,
                    TypeWarranty = item.TypeWarranty,
                    RegisterDate = item.RegisterDate,
                    ModelId = item.ModelId
                }).FirstOrDefaultAsync();
            return getItem!;
        }

        public async Task<ItemModel> GetModelAsync(int modelId)
        {
            var getModel = await _context.Item_Model.Where(m => m.Id.Equals(modelId))
                .Select(model => new ItemModel
                {
                    ModelName = model.ModelName,
                    Marker = model.Marker
                }).FirstOrDefaultAsync();
            return getModel!;
        }

        public async Task<ProductSupplierDTO> GetProductSupplier(int productId)
        {
            var detail = await GetDetails(productId);
            var supplier = await GetSupplierAsync(detail.InvoiceId);
            var item = await GetItemAsync(productId);
            var model = await GetModelAsync(item.ModelId);
            _productSupplierDTO = new()
            {
                PurchasePrice = detail.PurchasePrice,
                BussinesName = supplier.BussinesName,
                SupplierPhone = supplier.Phone,
                Manager = supplier.Manager,
                SupplierAddress = supplier.Address,
                SupplierNit = supplier.Ci,
                ItemName = item.Name,
                Serie = item.Serie,
                ItemDescription = item.Description,
                DurationWarranty = item.DurationWarranty,
                TypeWarranty = item.TypeWarranty,
                RegisterDateItem = item.RegisterDate,
                ItemModelName = model.ModelName,
                ItemMarker = model.Marker
            };
            return _productSupplierDTO;
        }
        #endregion

        #region ItemsBySupplierBetweenDates
        public async Task<SupplierItemsDTO> ItemsBySupplierBetweenDates(int supplierId, DateTime startDate, DateTime endDate)
        {
            DateTime newEndDate = endDate.AddDays(1);
            SupplierItemsDTO supplierItems = new();
            List<DeliveryDetail> details = new();

            var getInvoices = await _context.Invoices.Join(_context.DeliveryDetails, i => i.Id, dd => dd.InvoiceId,
                (i, dd) => new
                {
                    InvoiceRegisterDate = i.RegisterDate,
                    WhoSupplier = i.SupplierId,
                    DetailID = dd.Id,
                    dd.ItemId,
                    dd.PurchasePrice,
                    i.RegisterDate
                })
                .Where(i => i.WhoSupplier == supplierId && i.InvoiceRegisterDate >= startDate && i.InvoiceRegisterDate <= newEndDate)
                .ToListAsync();

            supplierItems = await GetSupplierBetweenDates(getInvoices.Select(gi => gi.WhoSupplier).First(), supplierItems);
            supplierItems.Products = new();
            
            foreach (var item in getInvoices)
            {
                var product = await GetItemAsync(item.ItemId);
                var productModel = await GetModelAsync(product.ModelId);
                supplierItems.Products.Add(new ProductPurchasedDTO
                {
                    NameProduct = product!.Name,
                    Serie = product.Serie,
                    PriceProduct = item.PurchasePrice,
                    MarkerProduct = productModel.Marker,
                    ModelNameProduct = productModel.ModelName,
                    RegisterDateProduct = item.RegisterDate
                });
            }
            return supplierItems;
        }

        public async Task<SupplierItemsDTO> GetSupplierBetweenDates(int supplierId, SupplierItemsDTO supplierItems)
        {
            var getSupplier = await _context.Suppliers.Where(s => s.Id == supplierId)
                .Select(s => new Supplier
                {
                    BussinesName = s.BussinesName,
                    Phone = s.Phone,
                    Manager = s.Manager,
                    Address = s.Address,
                    Ci = s.Ci
                }).FirstOrDefaultAsync();

            supplierItems.BussinesNameSupplier = getSupplier.BussinesName;
            supplierItems.PhoneSupplier = getSupplier.Phone;
            supplierItems.ManagerSupplier = getSupplier.Manager;
            supplierItems.AddressSupplier = getSupplier.Address;
            supplierItems.CiSupplier = getSupplier.Ci;
            return supplierItems;
        }
        #endregion
    }
}