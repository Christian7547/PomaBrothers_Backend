using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PomaBrothers.Models;

[Keyless]
[Table("Config")]
public partial class Config
{
    [Column("pathImageItem")]
    [StringLength(200)]
    [Unicode(false)]
    public string? PathImageItem { get; set; }
}
