using Microsoft.AspNetCore.Mvc;
using PomaBrothers.Reports.Interfaces;

namespace PomaBrothers.Controllers.Reports
{
    [Route("[controller]")]
    [ApiController]
    public class DeliveryReportsController : ControllerBase
    {
        private readonly IDeliveryReportsService _deliveryReportsService;

        public DeliveryReportsController(IDeliveryReportsService deliveryReportsService) => _deliveryReportsService = deliveryReportsService;

        [HttpGet, Route("GetOrdersByDateRangeReport")]
        public async Task<IActionResult> GetOrdersByDateRangeReport(DateTime startDate, DateTime endDate)
        {
            var getOrders = await _deliveryReportsService.OrdersByDateRangeReport(startDate, endDate);
            if(getOrders.Count > 0)
                return Ok(getOrders);
            return NotFound("No logs");
        }

        [HttpGet, Route("GetSupplierByProductId/{productId}")]
        public async Task<IActionResult> GetSupplierByProductId([FromRoute]int productId)
        {
            var getSupplierProduct = await _deliveryReportsService.GetProductSupplier(productId);
            if(getSupplierProduct != null) 
                return Ok(getSupplierProduct);
            return NotFound("No record");
        }
    }
}
