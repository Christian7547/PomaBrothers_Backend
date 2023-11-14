using PomaBrothers.Models;
using PomaBrothers.Models.DTOModels;

namespace PomaBrothers.Reports.Interfaces
{
    public interface ISalesReportsService
    {
        Task<Customer> GetCustomer(int customerId);
        Task<Sale> GetSale(int customerId);
        Task<List<SaleDetail>> GetDetails(int saleId);
        Task<List<Item>> GetItems(List<SaleDetail> saleDetails);
        Task<PurchasesCustomerDTO> GetPurchasesCustomerDTO(int customerId);
        Task<List<SaleDTO>> SalesByDateRangeReport(DateTime startDate, DateTime endDate);
        List<Item> ItemsJoinSaleDetails(List<SaleDetail> details);
        Task ItemsJoinItemModels(List<Item> items);
        SaleDTO MapperSaleToSaleDTO(Sale target, SaleDTO source);
        ProductPurchasedDTO MapperItemToProductPurchasedDTO(Item source);
    }
}