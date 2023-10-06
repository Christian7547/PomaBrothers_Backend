using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PomaBrothers.Models;

[Table("DeliveryDetail")]
public partial class DeliveryDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("itemId")]
    public int ItemId { get; set; }

    [Column("invoiceId")]
    public int InvoiceId { get; set; }

    [Column("purchasePrice", TypeName = "decimal(8, 2)")]
    public decimal PurchasePrice { get; set; }

    public virtual Item? Item { get; set; }
}
