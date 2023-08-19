using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
    public int CustomerId { get; set; }

    [Column("total", TypeName = "decimal(8, 2)")]
    public decimal Total { get; set; }

    [Column("registerDate", TypeName = "datetime")]
    public DateTime RegisterDate { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Sales")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("Sales")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("IdSaleNavigation")]
    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
