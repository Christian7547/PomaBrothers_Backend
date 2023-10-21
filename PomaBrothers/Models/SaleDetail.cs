using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PomaBrothers.Models;

public partial class SaleDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idSale")]
    [ForeignKey("Sale")]
    public int IdSale { get; set; }

    [Column("idItem")]
    [ForeignKey("Item")]
    public int IdItem { get; set; }

    [Column("subtotal", TypeName = "decimal(8, 2)")]
    public decimal Subtotal { get; set; }

    public virtual Item Item { get; set; } = null!;

    [JsonIgnore]
    public virtual Sale Sale { get; set; } = null!;
}
