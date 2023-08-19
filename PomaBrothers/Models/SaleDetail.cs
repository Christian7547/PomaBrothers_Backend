using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("subtotal", TypeName = "decimal(8, 2)")]
    public decimal Subtotal { get; set; }

    [ForeignKey("IdItem")]
    [InverseProperty("SaleDetails")]
    public virtual Item IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdSale")]
    [InverseProperty("SaleDetails")]
    public virtual Sale IdSaleNavigation { get; set; } = null!;
}
