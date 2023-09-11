using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PomaBrothers.Models
{
    public class ItemModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string ModelName { get; set; }

        [Column("marker")]
        [StringLength(50)]
        [Unicode(false)]
        public string Marker { get; set; } = null!;
    
        public int? CapacityOrSize { get; set; }

        [StringLength(50)]
        public string? MeasurementUnit { get; set; }
    }
}
