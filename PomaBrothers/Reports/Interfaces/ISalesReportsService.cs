using PomaBrothers.Models;

namespace PomaBrothers.Reports.Interfaces
{
    public interface ISalesReportsService
    {
        Task<List<Item>> ProductsPurchasedByCustomerId(int customerId);
        Task<List<Sale>> SalesByCustomer(int customerId);
    }
}
