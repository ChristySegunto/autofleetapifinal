using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table ("rented_vehicle")]
public class RentedVehicle {
    [Key]
    public int rented_vehicle_id { get; set; }
    public int renter_id { get; set; } // Foreign key to Renter table
    public int vehicle_id { get; set; } // Foreign key to Vehicle table
    public string renter_fname { get; set; }
    public string renter_lname { get; set; }
    public string pickup_loc { get; set; }
    public DateTime pickup_date { get; set; }
    public TimeSpan pickup_time { get; set; }
    public string dropoff_loc { get; set; }
    public DateTime dropoff_date { get; set; }
    public TimeSpan dropoff_time { get; set; }
    public string car_manufacturer { get; set; }
    public string car_model{ get; set; }
    public string plate_number { get; set; }
    public string rent_status { get; set; }

    [ForeignKey("renter_id")]
    public Renter Renter { get; set; }
    
    [ForeignKey("vehicle_id")]
    public Vehicle Vehicle { get; set; }
}