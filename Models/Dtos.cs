// DTO (Data Transfer Object) representing the rental schedule for a vehicle
public class RentedScheduleDto
{
    // The ID of the vehicle being rented
    public int VehicleId { get; set; }

    // The full name of the renter
    public string RenterName { get; set; }

    // The date and time when the vehicle is picked up
    public DateTime PickupDate { get; set; }

    // The date and time when the vehicle is dropped off
    public DateTime DropoffDate { get; set; }

    // The name or model of the vehicle being rented
    public string VehicleName { get; set; }

    // The current status of the rental
    public string Status { get; set; }
}

// DTO (Data Transfer Object) representing the maintenance schedule for a vehicle
public class MaintenanceScheduleDto
{
    // The ID of the vehicle that is undergoing maintenance
    public int VehicleId { get; set; }

    // The plate number of the vehicle being serviced
    public string PlateNumber { get; set; }

    // The model name of the vehicle being serviced
    public string CarModel { get; set; }

    // The type of maintenance being performed
    public string MaintenanceType { get; set; }

    // The current status of the maintenance task
    public string MaintenanceStatus { get; set; }

    // The due date for the maintenance task to be completed
    public DateTime MaintenanceDueDate { get; set; }
}

// A response object that contains both rental and maintenance schedules for today
public class TodaySchedulesResponse
{
    // A list of rental schedules for today (each entry is an instance of RentedScheduleDto)
    public List<RentedScheduleDto> RentalSchedules { get; set; }

    // A list of maintenance schedules for today (each entry is an instance of MaintenanceScheduleDto)
    public List<MaintenanceScheduleDto> MaintenanceSchedules { get; set; }
}
