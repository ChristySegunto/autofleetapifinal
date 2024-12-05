using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class RentedVehicle
{
    // Unique identifier for the rented vehicle record
    [Key]
    public int rented_vehicle_id { get; set; }

    // Foreign Key for Renter
    public int renter_id { get; set; }

    // Foreign Key for Vehicle
    public int vehicle_id { get; set; }

    // Renter Information (First and Last Name)
    public string renter_fname { get; set; }
    public string renter_lname { get; set; }

    // Rental Dates and Times
    // The date when the vehicle is picked up
    [Required]
    public DateTime pickup_date { get; set; }

    // The time when the vehicle is picked up
    [Required]
    public TimeSpan pickup_time { get; set; }

    // The date when the vehicle is dropped off
    [Required]
    public DateTime dropoff_date { get; set; }

    // The time when the vehicle is dropped off
    [Required]
    public TimeSpan dropoff_time { get; set; }

    // Vehicle Details (Derived from the Vehicle object)
    public string car_manufacturer { get; set; }
    public string car_model { get; set; }
    public string plate_number { get; set; }

    // Metadata indicating when the rental record was created
    public DateTime created_at { get; set; } = DateTime.UtcNow;

    // Metadata indicating when the rental record was last updated
    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    // A derived property to combine the renter's first and last name
    // This is not stored in the database, but it is useful for displaying the full renter's name
    [NotMapped]
    public string RenterName => $"{renter_fname} {renter_lname}";

    // A collection of car updates (real-time information) related to this rented vehicle
    // This collection will not be serialized into JSON responses
    [JsonIgnore] 
    public ICollection<CarUpdate>? CarUpdates { get; set; } 

    // The current rental status
    // Default is "Upcoming" to indicate the rental hasn't started yet
    [MaxLength(20)]
    public string rent_status { get; set; } = "Upcoming"; // Default status
}
