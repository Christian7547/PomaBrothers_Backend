using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PomaBrothers.Models;

[Table("Sale")]
public partial class Sale
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("employeeId")]
    public int EmployeeId { get; set; }

    [Column("customerId")]
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }

    [Column("total", TypeName = "decimal(8, 2)")]
    public decimal Total { get; set; }

    [Column("registerDate", TypeName = "datetime")]
    public DateTime RegisterDate { get; set; }

    public virtual Customer? Customer { get; set; } = null!;

    [JsonIgnore]
    public virtual Employee? Employee { get; set; } = null!;

    public virtual ICollection<SaleDetail>? SaleDetails { get; set; } = new List<SaleDetail>();
}
