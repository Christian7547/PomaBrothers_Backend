using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PomaBrothers.Models;

[Table("Customer")]
public partial class Customer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("lastName")]
    [StringLength(10)]
    public string LastName { get; set; } = null!;

    [Column("secondLastName")]
    [StringLength(10)]
    public string? SecondLastName { get; set; }

    [Column("email")]
    [StringLength(60)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("ci")]
    [StringLength(50)]
    [Unicode(false)]
    public string Ci { get; set; } = null!;

    [Column("registerDate", TypeName = "datetime")]
    public DateTime RegisterDate { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
