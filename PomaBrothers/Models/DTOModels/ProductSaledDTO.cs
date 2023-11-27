namespace PomaBrothers.Models.DTOModels
{
    public class ProductSaledDTO
    {
        public int ProductId { get; set; }
        public int ModelId { get; set; }
        public decimal ProductPrice { get; set; }
        public byte Status { get; set; }
    }
}
