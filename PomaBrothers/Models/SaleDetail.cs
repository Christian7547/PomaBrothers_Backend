using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PomaBrothers.Models;

public partial class SaleDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idSale")]
    public int IdSale { get; set; }

    [Column("idItem")]
    public int IdItem { get; set; }

    [Column("subtotal", TypeName = "decimal(8, 2)")]
    public decimal Subtotal { get; set; }

    [ForeignKey("IdItem")]
    [InverseProperty("SaleDetails")]
    public virtual Item? IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdSale")]
    [InverseProperty("SaleDetails")]
    public virtual Sale? IdSaleNavigation { get; set; } = null!;
}
