using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("report")]
public class Report
{
    [Key]
    public int report_id { get; set; } // Unique identifier for the report

    public int renter_id { get; set; } // Foreign key referring to the renter who created the report

    // Description of the nature of the issue being reported
    [Required]
    [StringLength(500)] // Limits the nature of the issue to 500 characters
    public string nature_of_issue { get; set; }

    public DateTime date { get; set; } // The date when the issue was reported
    public TimeSpan time { get; set; } // The time when the issue was reported

    public string note { get; set; } // Optional additional note or comment about the report

    public string emergency { get; set; } // Flag indicating whether the report is related to an emergency

    public DateTime created_at { get; set; } = DateTime.UtcNow; // Timestamp when the report was created (defaults to UTC now)

    public DateTime updated_at { get; set; } = DateTime.UtcNow; // Timestamp when the report was last updated (defaults to UTC now)

}
