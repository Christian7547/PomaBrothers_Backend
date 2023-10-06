using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PomaBrothers.Models;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("supplierId")]
    public int SupplierId { get; set; }

    [Column("registerDate", TypeName = "datetime")]
    public DateTime RegisterDate { get; set; } = DateTime.Now;

    [Column("total", TypeName = "decimal(8, 2)")]
    public decimal Total { get; set; }

    [ForeignKey("SupplierId")]
    public virtual Supplier? Supplier { get; set; }

    public virtual ICollection<DeliveryDetail>? DeliveryDetails { get; set; } = new List<DeliveryDetail>();
}
