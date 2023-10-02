using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PomaBrothers.Models;

[Table("Item")]
public partial class Item
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(200)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("serie")]
    [StringLength(50)]
    [Unicode(false)]
    public string Serie { get; set; } = null!;

    [Column("description")]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    [Column("price", TypeName = "decimal(8, 2)")]
    public decimal Price { get; set; }

    [Column("color")]
    [StringLength(50)]
    [Unicode(false)]
    public string Color { get; set; } = null!;

    /// <summary>
    /// Años de garantía
    /// </summary>
    [Column("warranty")]
    [StringLength(10)]
    [Unicode(false)]
    public string Warranty { get; set; } = null!;

    [Column("categoryId")]
    public int CategoryId { get; set; }

    [Column("registerDate", TypeName = "datetime")]
    public DateTime RegisterDate { get; set; }

    [Column("modelId")]
    public int ModelId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Items")]
    public virtual Category? Category { get; set; } = null!;

    [ForeignKey("ModelId")]
    public virtual ItemModel? ItemModel { get; set; } = null!;

    [InverseProperty("Item")]
    public virtual ICollection<DeliveryDetail>? DeliveryDetails { get; set; } = new List<DeliveryDetail>();

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<SaleDetail>? SaleDetails { get; set; } = new List<SaleDetail>();

    [InverseProperty("Item")]
    public virtual ICollection<Section>? Sections { get; set; } = new List<Section>();
}
