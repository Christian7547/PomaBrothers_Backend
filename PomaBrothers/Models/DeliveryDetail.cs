using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PomaBrothers.Models;

[Table("DeliveryDetail")]
public partial class DeliveryDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("supplierId")]
    public int SupplierId { get; set; }

    [Column("itemId")]
    public int ItemId { get; set; }

    [Column("invoiceId")]
    public int InvoiceId { get; set; }

    [Column("purchasePrice", TypeName = "decimal(8, 2)")]
    public decimal PurchasePrice { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("subTotal", TypeName = "decimal(8, 2)")]
    public decimal SubTotal { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("DeliveryDetails")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("DeliveryDetails")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("SupplierId")]
    [InverseProperty("DeliveryDetails")]
    public virtual Supplier Supplier { get; set; } = null!;
}
