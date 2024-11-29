using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Table("vehicle")]
public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int vehicle_id { get; set; }
    public string car_manufacturer { get; set; }
    public string car_model { get; set; }
    public string plate_number { get; set; }
    public int? manufacture_year { get; set; }
    public string vehicle_color { get; set; }
    public string fuel_type { get; set; }
    public string transmission_type { get; set; }
    public int? seating_capacity { get; set; }
    public string vehicle_category { get; set; }
    public decimal total_mileage { get; set; }
    public decimal total_fuel_consumption { get; set; }
    public decimal distance_traveled { get; set; }
    public string vehicle_status { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}