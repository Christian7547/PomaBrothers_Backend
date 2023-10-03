namespace PomaBrothers.Models.DTOModels
{
    public class DeliveryDTO
    {
        public int SupplierId { get; set; }
        public double Total { get; set; }
        public List<Item> Items { get; set; }
        public List<decimal> PurchasePrices { get; set; }
        public int WarehouseId { get; set; }
    }
}
