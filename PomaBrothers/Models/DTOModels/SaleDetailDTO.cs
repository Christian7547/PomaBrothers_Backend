using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace PomaBrothers.Models.DTOModels
{
    public class SaleDetailDTO
    {
        public int EmployeeId { get; set; }

        public int CustomerId { get; set; }

        public decimal Total { get; set; }

        public DateTime RegisterDate { get; set; }

        public List<ProductSaledDTO> ProductSaled { get; set; }
    }
}
