using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Table("vehicle")]
public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int vehicle_id { get; set; } // Unique identifier for the vehicle (auto-incremented)
    public string car_manufacturer { get; set; } // Manufacturer of the vehicle
    public string car_model { get; set; } // Model of the vehicle
    public string plate_number { get; set; } // The vehicle's license plate number
    public int? manufacture_year { get; set; } // The year the vehicle was manufactured
    public string vehicle_color { get; set; } // Color of the vehicle
    public string fuel_type { get; set; } // Type of fuel the vehicle uses
    public string transmission_type { get; set; } // Type of transmission
    public int? seating_capacity { get; set; } // The number of people the vehicle can seat 
    public string vehicle_category { get; set; } // Category of the vehicle 
    public decimal? total_mileage { get; set; } // Total distance the vehicle has traveled
    public decimal? total_fuel_consumption { get; set; } // Total fuel consumption of the vehicle
    public decimal? distance_traveled { get; set; } // Distance traveled by the vehicle in the current rental
    public string vehicle_status { get; set; } // Current status of the vehicle
    public DateTime created_at { get; set; } // Timestamp when the vehicle record was created
    public DateTime updated_at { get; set; } // Timestamp when the vehicle record was last updated
}