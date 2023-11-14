using Microsoft.AspNetCore.Mvc;
using PomaBrothers.Reports.Interfaces;

namespace PomaBrothers.Controllers.ReportsControllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalesReportsController : ControllerBase
    {
        private readonly ISalesReportsService _salesReportsService;

        public SalesReportsController(ISalesReportsService salesReportsService) 
        {
            _salesReportsService = salesReportsService;
        }

        [HttpGet, Route("GetPurchasesCustomerReport/{customerId}")]
        public async Task<IActionResult> GetPurchasesCustomerReport([FromRoute]int customerId)
        {
            var getPurchasesByCustomer = await _salesReportsService.GetPurchasesCustomerDTO(customerId);
            if(getPurchasesByCustomer != null)
                return Ok(getPurchasesByCustomer);
            return NotFound("The customer doesn't exist");
        }

        [HttpGet, Route("GetSalesRangeReport")]
        public async Task<IActionResult> GetSalesByDateRangeReport(DateTime startDate, DateTime endDate)
        {
            var getSales = await _salesReportsService.SalesByDateRangeReport(startDate, endDate);
            if (getSales.Count > 0)
                return Ok(getSales);
            return NotFound("No logs");
        }
    }
}
