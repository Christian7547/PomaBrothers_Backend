using Microsoft.EntityFrameworkCore;
using PomaBrothers.Data;
using PomaBrothers.Models;
using PomaBrothers.Models.DTOModels;
using PomaBrothers.Reports.Interfaces;
using System.Text.RegularExpressions;

namespace PomaBrothers.Reports.Implementation
{
    public class SalesReportsService : ISalesReportsService
    {
        private PurchasesCustomerDTO? _puchasesCustomerDTO;
        private readonly PomaBrothersDbContext _context;

        public SalesReportsService(PomaBrothersDbContext context)
        {
            _context = context;
        }
        #region PurchasesByCustomerId
        public async Task<Customer> GetCustomer(int customerId)
        {
            var getCustomer = await _context.Customers.Where(x => x.Id == customerId)
                .Select(customer => new Customer
                {
                    Name = customer.Name,
                    LastName = customer.LastName,
                    SecondLastName = customer.SecondLastName,
                    Ci = customer.Ci,
                    Email = customer.Email
                }).FirstOrDefaultAsync();
            return getCustomer != null ? getCustomer : null!;
        }

        public async Task<Sale> GetSale(int customerId)
        {
            var getSale = await _context.Sales.Where(x => x.CustomerId == customerId)
                .Select(sale => new Sale
                {
                    Total = sale.Total,
                    RegisterDate = sale.RegisterDate,
                    Id = sale.Id
                }).FirstOrDefaultAsync();
            return getSale != null ? getSale : null!;
        }

        public async Task<List<SaleDetail>> GetDetails(int saleId)
        {
            var getdetails = await _context.SaleDetails.Where(gd => gd.IdSale == saleId)
                .Select(detail => new SaleDetail
                {
                    IdSale = detail.IdSale,
                    IdItem = detail.IdItem,
                    Subtotal = detail.Subtotal
                }).ToListAsync();
            return getdetails;
        }

        public async Task<List<Item>> GetItems(List<SaleDetail> saleDetails)
        {
            var itemIds = saleDetails.Select(sd => sd.IdItem).ToList();
            var items = await _context.Items
                .Where(i => itemIds.Contains(i.Id))
                .Include(i => i.ItemModel)
                .ToListAsync();

            var getItems = items.Select(i => new Item
            {
                Name = i.Name,
                Serie = i.Serie,
                Price = saleDetails.First(sd => sd.IdItem == i.Id).Subtotal,
                ItemModel = items.Where(i => i.ItemModel!.Id == i.ModelId).Select(i => i.ItemModel).First()
            }).ToList();

            return getItems;
        }

        public async Task<PurchasesCustomerDTO> GetPurchasesCustomerDTO(int customerId)
        {
            _puchasesCustomerDTO = new();
            List<ProductPurchasedDTO> products = new();
            var customer = await GetCustomer(customerId);
            if (customer.SecondLastName != null)
                _puchasesCustomerDTO.CompleteNameCustomer = String.Concat(customer.Name, " ", customer.LastName, " ", customer.SecondLastName);
            else
                _puchasesCustomerDTO.CompleteNameCustomer = String.Concat(customer.Name," ", customer.LastName);
            _puchasesCustomerDTO.CompleteNameCustomer = Regex.Replace(_puchasesCustomerDTO.CompleteNameCustomer, @"\s+", " ").Trim();
            var sale = await GetSale(customerId);
            var details = await GetDetails(sale.Id);
            var getProducts = await GetItems(details);

            foreach(var item in getProducts)
            {
                var purchasedProduct = new ProductPurchasedDTO()
                {
                    NameProduct = item.Name,
                    Serie = item.Serie,
                    PriceProduct = item.Price,
                    MarkerProduct = item.ItemModel!.Marker,
                    ModelNameProduct = item.ItemModel.ModelName
                };
                products.Add(purchasedProduct);
            }
            _puchasesCustomerDTO.SaleDTO = new();
            _puchasesCustomerDTO.CiCustomer = customer.Ci;
            _puchasesCustomerDTO.EmailCustomer = customer.Email;
            _puchasesCustomerDTO.SaleDTO.Total = sale.Total;
            _puchasesCustomerDTO.SaleDTO.RegisterDate = sale.RegisterDate;
            _puchasesCustomerDTO.SaleDTO.Products = products;
            return _puchasesCustomerDTO;
        }
        #endregion

        #region SalesByDateRange
        public async Task<List<SaleDTO>> SalesByDateRangeReport(DateTime startDate, DateTime endDate)
        {
            DateTime newEndDate = endDate.AddDays(1);
            var salesDTO = new List<SaleDTO>();
            var getSales = await _context.Sales.Where(s => s.RegisterDate.CompareTo(startDate) >= 0 && s.RegisterDate.CompareTo(newEndDate) < 0)
                .Include(details => details.SaleDetails)
                .ToListAsync();
            foreach(var sale in getSales)
            {
                var listItems = ItemsJoinSaleDetails(sale.SaleDetails!.ToList());
                salesDTO = getSales.Select(sale => new SaleDTO { SaleId = sale.Id }).ToList();
            }

            foreach (var dto in salesDTO)
            {
                var bindSale = getSales.Where(s => s.Id == dto.SaleId).FirstOrDefault();
                MapperSaleToSaleDTO(bindSale!, dto);
            }
            return salesDTO!;
        }

        public List<Item> ItemsJoinSaleDetails(List<SaleDetail> details)
        {
            var foreignDetail = details.Select(d => new SaleDetail { IdItem = d.IdItem, Subtotal = d.Subtotal }).ToList();
            var items = foreignDetail.Join(_context.Items, fd => fd.IdItem, i => i.Id,
                (fd, i) => new Item
                {
                    Name = i.Name,
                    Serie = i.Serie,
                    Price = fd.Subtotal
                })
                .ToList();
            ItemsJoinItemModels(items);
            return items;
        }

        public Task ItemsJoinItemModels(List<Item> items)
        {
            items = items.Join(_context.Item_Model, i => i.ModelId, model => model.Id,
                (i, model) => new Item
                {
                    Name = i.Name,
                    Serie = i.Serie,
                    Price = i.Price,
                    ItemModel = new ItemModel { Marker = model.Marker, ModelName = model.ModelName }
                }).ToList();
            return Task.CompletedTask;
        }

        public SaleDTO MapperSaleToSaleDTO(Sale target, SaleDTO source)
        {
            source.Products = new List<ProductPurchasedDTO>();
            source.Total = target.Total;
            source.RegisterDate = target.RegisterDate;
            foreach(var items in target.SaleDetails!)
                source.Products.Add(MapperItemToProductPurchasedDTO(items.Item));
            return source;
        }

        public ProductPurchasedDTO MapperItemToProductPurchasedDTO(Item source)
        {
            return new ProductPurchasedDTO
            {
                NameProduct = source.Name,
                ModelNameProduct = source.ItemModel!.ModelName,
                MarkerProduct = source.ItemModel.Marker,
                Serie = source.Serie,
                PriceProduct = source.Price
            };
        }
        #endregion
    }
}