using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PomaBrothers.Models;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("registerDate", TypeName = "datetime")]
    public DateTime RegisterDate { get; set; }

    [Column("total", TypeName = "decimal(8, 2)")]
    public decimal Total { get; set; }

    [Column("deliveryDetailId")]
    public int DeliveryDetailId { get; set; }

    [InverseProperty("Invoice")]
    public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; } = new List<DeliveryDetail>();
}
