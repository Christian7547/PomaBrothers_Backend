using PomaBrothers.Models;
using PomaBrothers.Models.DTOModels;

namespace PomaBrothers.Reports.Interfaces
{
    public interface IDeliveryReportsService
    {
        Task<List<DeliveryDetail>> OrdersByDateRangeReport(DateTime startDate, DateTime endDate);
        Task<ProductSupplierDTO> GetProductSupplier(int productId);
        Task<Item> GetItemAsync(int productId);
        Task<DeliveryDetail> GetDetails(int productId);
        Task<Supplier> GetSupplierAsync(int invoiceId);
        Task<ItemModel> GetModelAsync(int modelId);
        Task<SupplierItemsDTO> ItemsBySupplierBetweenDates(int supplierId, DateTime startDate, DateTime endDate);
    }
}
