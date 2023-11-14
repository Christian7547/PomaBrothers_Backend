namespace PomaBrothers.Models.DTOModels
{
    public class ProductSupplierDTO
    {
        public string ItemName { get; set; }
        public string ItemModelName { get; set; }
        public string ItemMarker { get; set; }
        public string Serie { get; set; }
        public string ItemDescription { get; set; }
        public decimal PurchasePrice { get; set; }
        public byte DurationWarranty { get; set; }
        public string TypeWarranty { get; set; }
        public DateTime RegisterDateItem { get; set; }
        public string BussinesName { get; set; }
        public string SupplierPhone { get; set; }
        public string Manager { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierNit { get; set; }
    }
}
