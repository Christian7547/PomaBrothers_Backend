using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PomaBrothers.Models;

[Table("Employee")]
public partial class Employee
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("lastName")]
    [StringLength(50)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [Column("secondLastName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SecondLastName { get; set; }

    /// <summary>
    /// Carnet de identidad
    /// </summary>
    [Column("ci")]
    [StringLength(20)]
    [Unicode(false)]
    public string Ci { get; set; } = null!;

    [Column("phone")]
    [StringLength(10)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [Column("user")]
    [StringLength(50)]
    [Unicode(false)]
    public string User { get; set; } = null!;

    [Column("password")]
    [StringLength(50)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Column("role")]
    [StringLength(15)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    [Column("registerDate", TypeName = "datetime")]
    public DateTime RegisterDate { get; set; } = DateTime.Now;

    [Column("urlImage")]
    [StringLength(500)]
    [Unicode(false)]
    public string? UrlImage { get; set; }



    [InverseProperty("Employee")]
    public virtual ICollection<Sale>? Sales { get; set; } = new List<Sale>();
}
