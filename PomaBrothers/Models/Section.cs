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

    [Column("modelId")]
    public int ModelId { get; set; }

    [Column("modelQuantity")]
    public int ModelQuantity { get; set; }

    [ForeignKey("ModelId")]
    public virtual ItemModel? ItemModel { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("Sections")]
    public virtual Warehouse? Warehouse { get; set; } = null!;
}
