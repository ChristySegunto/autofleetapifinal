using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table ("realtime_carupdate")] // Specifies that this class maps to the "realtime_carupdate" table in the database
public class CarUpdate
{
    [Key] // Specifies that carupdate_id is the primary key of the "realtime_carupdate" table
    public int carupdate_id { get; set; }

    public int renter_id { get; set; } // Foreign key to the Renter table (links to a specific renter)
    [ForeignKey("renter_id")] // Specifies that renter_id is a foreign key to the Renter table
    public Renter? Renter { get; set; } // Navigation property for the associated Renter entity
    public string renter_fname { get; set; } // First name of the renter
    public string renter_lname { get; set; } // Last name of the renter
    public decimal? location_latitude { get; set; } // Latitude of the vehicle's current location (nullable)
    public decimal? location_longitude { get; set; } // Longitude of the vehicle's current location (nullable)
    public decimal? speed { get; set; } // Current speed of the vehicle (nullable)
    public decimal? total_fuel_consumption { get; set; } // Total fuel consumed by the vehicle (nullable)
    public decimal? total_distance_travelled { get; set; } // Total distance traveled by the vehicle (nullable)
    public DateTime last_update { get; set; } // Timestamp for the last update of the vehicle's data



    public int vehicle_id { get; set; } // Foreign key to the Vehicle table (links to a specific vehicle)
    public int rented_vehicle_id { get; set; } // Foreign key to the RentedVehicle table (links to a specific rented vehicle)
    
    [ForeignKey("renter_id")] // Specifies that renter_id is a foreign key to the RentedVehicle table
    public RentedVehicle? RentedVehicle { get; set; } // Navigation property for the associated RentedVehicle entity


    public string carupdate_status { get; set; } // Status of the car update

}