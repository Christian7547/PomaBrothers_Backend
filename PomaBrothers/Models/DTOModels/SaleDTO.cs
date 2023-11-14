namespace PomaBrothers.Models.DTOModels
{
    public class SaleDTO
    {
        public int SaleId { get; set; }
        public decimal Total { get; set; }
        public DateTime RegisterDate { get; set; }
        public List<ProductPurchasedDTO> Products { get; set; }
    }
}
