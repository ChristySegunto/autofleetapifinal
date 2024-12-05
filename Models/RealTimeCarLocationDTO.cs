// DTO (Data Transfer Object) representing real-time car location and stats
public class RealTimeCarLocationDTO
{
    // The latitude of the vehicle's current location (in decimal degrees)
    public decimal LocationLatitude { get; set; }

    // The longitude of the vehicle's current location (in decimal degrees)
    public decimal LocationLongitude { get; set; }

    // The speed of the vehicle (in kilometers per hour, km/h)
    public decimal Speed { get; set; } // in km/h

    // The total fuel consumption of the vehicle (in Liters)
    public decimal TotalFuelConsumption { get; set; } // in Liters

    // The total distance traveled by the vehicle (in kilometers)
    public decimal TotalDistanceTravelled { get; set; } // in kilometers

    // The current status of the car update
    public string CarUpdateStatus { get; set; } 

}