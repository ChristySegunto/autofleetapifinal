public class RealTimeCarLocationDTO
{
    public decimal LocationLatitude { get; set; }
    public decimal LocationLongitude { get; set; }
    public decimal Speed { get; set; } // in km/h
    public decimal TotalFuelConsumption { get; set; } // in Liters
    public decimal TotalDistanceTravelled { get; set; } // in kilometers
}