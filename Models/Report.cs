using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("report")]
public class Report
{
    [Key]
    public int report_id { get; set; }

    public int renter_id { get; set; }

    [Required]
    [StringLength(500)]
    public string nature_of_issue { get; set; }

    public DateTime date { get; set; }
    public TimeSpan time { get; set; }

    public string note { get; set; }

    public string emergency { get; set; }

    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    // [ForeignKey("renter_id")]
    // public Renter Renter { get; set; }
}
