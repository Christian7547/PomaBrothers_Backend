namespace PomaBrothers.Models.DTOModels
{
    public class SupplierItemsDTO
    {
        public string BussinesNameSupplier { get; set; } 
        public string PhoneSupplier { get; set; } 
        public string ManagerSupplier { get; set; } 
        public string AddressSupplier { get; set; } 
        public string CiSupplier { get; set; }
        public List<ProductPurchasedDTO> Products { get; set; }
    }
}
