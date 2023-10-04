using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PomaBrothers.Models;

[Table("Supplier")]
public partial class Supplier
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("bussinesName")]
    [StringLength(50)]
    [Unicode(false)]
    public string BussinesName { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [Column("manager")]
    [StringLength(150)]
    [Unicode(false)]
    public string Manager { get; set; } = null!;

    [Column("address")]
    [StringLength(200)]
    [Unicode(false)]
    public string Address { get; set; } = null!;

    /// <summary>
    /// Carnet de identidad
    /// </summary>
    [Column("ci")]
    [StringLength(50)]
    [Unicode(false)]
    public string Ci { get; set; } = null!;

    [Column("registerDate", TypeName = "datetime")]
    public DateTime RegisterDate { get; set; }

    [InverseProperty("Supplier")]
    public virtual ICollection<Invoice>? Invoices { get; set; } = new List<Invoice>();
}
