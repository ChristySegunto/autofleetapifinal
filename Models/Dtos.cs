public class RentedScheduleDto
{
    public int VehicleId { get; set; }
    public string RenterName { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime DropoffDate { get; set; }
    public string VehicleName { get; set; }
    public string Status { get; set; }
}

public class MaintenanceScheduleDto
{
    public int VehicleId { get; set; }
    public string PlateNumber { get; set; }
    public string CarModel { get; set; }
    public string MaintenanceType { get; set; }
    public string MaintenanceStatus { get; set; }
    public DateTime MaintenanceDueDate { get; set; }
}

public class TodaySchedulesResponse
{
    public List<RentedScheduleDto> RentalSchedules { get; set; }
    public List<MaintenanceScheduleDto> MaintenanceSchedules { get; set; }
}
