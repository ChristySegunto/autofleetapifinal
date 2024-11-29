using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table ("realtime_carupdate")]
public class CarUpdate
{
    [Key]
    public int carupdate_id { get; set; }

    public int renter_id { get; set; }
    [ForeignKey("renter_id")]
    public Renter? Renter { get; set; }
    public string renter_fname { get; set; }
    public string renter_lname { get; set; }
    public decimal location_latitude { get; set; }
    public decimal location_longitude { get; set; }
    public decimal speed { get; set; }
    public decimal total_fuel_consumption { get; set; }
    public decimal total_distance_travelled { get; set; }
    public DateTime last_update { get; set; }



    public int vehicle_id { get; set; }

    // [ForeignKey("vehicle_id")]
    // public Vehicle Vehicle { get; set; }
    public string carupdate_status { get; set; }
}