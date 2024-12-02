using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class RentedVehicle
{
    [Key]
    public int rented_vehicle_id { get; set; }

    // Foreign Key for Renter
    public int renter_id { get; set; }

    // Foreign Key for Vehicle
    public int vehicle_id { get; set; }

    // Renter Information (Derived from the Renter object)
    public string renter_fname { get; set; }
    public string renter_lname { get; set; }

    // Rental Dates and Times
    [Required]
    public DateTime pickup_date { get; set; }

    [Required]
    public TimeSpan pickup_time { get; set; }

    [Required]
    public DateTime dropoff_date { get; set; }

    [Required]
    public TimeSpan dropoff_time { get; set; }

    // Vehicle Details (Derived from the Vehicle object)
    public string car_manufacturer { get; set; }
    public string car_model { get; set; }
    public string plate_number { get; set; }

    // Metadata
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public string RenterName => $"{renter_fname} {renter_lname}";


    [JsonIgnore] 
    public ICollection<CarUpdate> CarUpdates { get; set; } 


    [MaxLength(20)]
    public string rent_status { get; set; } = "Pending"; // Default status
}
