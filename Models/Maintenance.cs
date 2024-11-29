using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("maintenance")]
public class Maintenance
{
    [Key]
    public int maintenance_id { get; set; }  // Primary key - maintenance_id
    public int vehicle_id { get; set; }       // Foreign key - vehicle_id
    public string plate_num { get; set; }     // plate_num
    public string car_model { get; set; }     // car_model
    public string maintenance_type { get; set; }  // maintenance_type
    public string maintenance_status { get; set; }  // maintenance_status
    public DateTime maintenance_due_date { get; set; }  // maintenance_due_date
    public DateTime created_at { get; set; } = DateTime.UtcNow; // Valid default
    public DateTime updated_at { get; set; } = DateTime.UtcNow; // Valid default

    public DateTime maintenance_next_due_date { get; set; }  // maintenance_due_date

    // Navigation property to the Vehicle entity (if needed)
    // [ForeignKey("vehicle_id")]
    // public Vehicle Vehicle { get; set; }  
}
