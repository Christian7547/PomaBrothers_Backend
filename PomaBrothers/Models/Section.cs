using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PomaBrothers.Models;

[Table("Section")]
public partial class Section
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("warehouseId")]
    public int WarehouseId { get; set; }

    [Column("itemId")]
    public int ItemId { get; set; }

    [Column("itemQuantity")]
    public int ItemQuantity { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("Sections")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("Sections")]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
