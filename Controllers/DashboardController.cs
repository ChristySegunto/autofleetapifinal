using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class DashboardController : ControllerBase 
    {

        private readonly AutoFleetDbContext _context;

        public DashboardController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Dashboard/get-admin-details
        [HttpGet("get-admin-details")]
        public async Task<IActionResult> GetLoggedInAdmin([FromQuery] int userId)
        {
            try
            {


                if (userId <= 0) // Check if the userId is valid
                {
                    return BadRequest("Invalid user ID.");
                }

                // Directly query the Admin table based on user_id
                var admin = await _context.Admins
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.user_id == userId);

                if (admin == null || admin.User == null)
                {
                    return NotFound("Admin or associated user not found.");
                }

                return Ok(new
                {
                    AdminId = admin.admin_id,
                    FirstName = admin.admin_fname,
                    LastName = admin.admin_lname,
                    Email = admin.User.Email,
                    Role = admin.User.Role
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("vehicleslist")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            try
            {
                var vehicles = await _context.Vehicles.ToListAsync();
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/Dashboard/Count
        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetTotalVehiclesCount()
        {
            try
            {
                // Get the count of total vehicles
                int totalCount = await _context.Vehicles.CountAsync();
                return Ok(totalCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/Dashboard/StatusCount
        [HttpGet("StatusCount")]
        public async Task<ActionResult<object>> GetVehiclesStatusCount()
        {
            try
            {
                // Count for "Available"
                int availableCount = await _context.Vehicles
                    .Where(v => v.vehicle_status == "Available")
                    .CountAsync();

                // Count for "Rented"
                int rentedCount = await _context.Vehicles
                    .Where(v => v.vehicle_status == "Rented")
                    .CountAsync();

                // Count for "Under Maintenance"
                int underMaintenanceCount = await _context.Vehicles
                    .Where(v => v.vehicle_status == "Under Maintenance")
                    .CountAsync();

                var statusCounts = new
                {
                    Available = availableCount,
                    Rented = rentedCount,
                    UnderMaintenance = underMaintenanceCount
                };

                return Ok(statusCounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/Vehicles/CategoryCount
        [HttpGet("CategoryCount")]
        public async Task<ActionResult<object>> GetVehiclesCategoryCount()
        {
            try
            {
                // Count for "SUV"
                int suvCount = await _context.Vehicles
                    .Where(v => v.vehicle_category == "SUV")
                    .CountAsync();

                // Count for "Van"
                int vanCount = await _context.Vehicles
                    .Where(v => v.vehicle_category == "Van")
                    .CountAsync();

                // Count for "Sedan"
                int sedanCount = await _context.Vehicles
                    .Where(v => v.vehicle_category == "Sedan")
                    .CountAsync();

                var categoryCounts = new
                {
                    SUV = suvCount,
                    Van = vanCount,
                    Sedan = sedanCount
                };

                return Ok(categoryCounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("RecentBookings")]
        public async Task<ActionResult<IEnumerable<RentedVehicle>>> GetRecentBookings()
        {
            try
            {
                // Fetch the recent bookings from the database, sorted by StartDate
                var recentBookings = await _context.RentedVehicles
                    .OrderByDescending(b => b.pickup_date) // Sort by StartDate to get recent bookings first
                    .Select(b => new
                    {
                        b.vehicle_id,
                        b.renter_fname,
                        pickup_date = b.pickup_date.Date, // Extract only the date part (no time)
                        dropoff_date = b.dropoff_date.Date,
                        b.rent_status
                    })
                    .ToListAsync();

                if (recentBookings == null || !recentBookings.Any())
                {
                    return NotFound("No recent bookings found.");
                }

                // Get today's date for comparison
                var today = DateTime.Today;


                // Check for bookings with a pickup_date that is today or in the future and update vehicle status
                foreach (var booking in recentBookings)
                {
                    if (booking.pickup_date >= today)  // If pickup_date is today or in the future
                    {
                        var vehicle = await _context.Vehicles
                            .FirstOrDefaultAsync(v => v.vehicle_id == booking.vehicle_id);

                        if (vehicle != null)
                        {
                            vehicle.vehicle_status = "Rented";  // Update the vehicle status
                        }

                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return the updated bookings and their statuses, but limit to 5 when displaying
                // return Ok(new 
                // {
                //     RecentBookings = recentBookings.Select(booking => new 
                //     {
                //         booking.vehicle_id,
                //         booking.renter_fname,
                //         booking.pickup_date,
                //         booking.dropoff_date,
                //         booking.rent_status,
                //         vehicle_status = _context.Vehicles
                //             .Where(v => v.vehicle_id == booking.vehicle_id)
                //             .Select(v => v.vehicle_status)
                //             .FirstOrDefault()
                //     })                
                // });

                return Ok(recentBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("get-today-schedules")]
        public IActionResult GetTodaySchedules()
        {
            try
            {
                // Get today's date
                var today = DateTime.Today;
                var yesterday = today.AddDays(-1);


                // Fetch rental schedules for today
                var rentalSchedules = _context.RentedVehicles
                    .Where(r => r.pickup_date.Date == today)
                    .Select(r => new RentedScheduleDto
                    {
                        VehicleId = r.vehicle_id,
                        RenterName = r.renter_fname + " " + r.renter_lname, // Concatenate the names
                        PickupDate = r.pickup_date,
                        VehicleName = r.car_manufacturer + " " + r.car_model,
                        Status = r.rent_status,

                    })
                    .ToList();

                // Fetch maintenance schedules for today
                var maintenanceSchedules = _context.Maintenances
                    .Where(m => m.maintenance_due_date.Date == today || m.maintenance_due_date.Date == yesterday) // Filter maintenance by today's due date
                    .Select(m => new MaintenanceScheduleDto
                    {
                        VehicleId = m.vehicle_id,
                        PlateNumber = m.plate_num,
                        CarModel = m.car_model,
                        MaintenanceType = m.maintenance_type,
                        MaintenanceStatus = m.maintenance_status,
                        MaintenanceDueDate = m.maintenance_due_date
                    })
                    .ToList();

                // Prepare the response
                var response = new TodaySchedulesResponse
                {
                    RentalSchedules = rentalSchedules,
                    MaintenanceSchedules = maintenanceSchedules
                };

                return Ok(response); // Return the schedules in the response
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Reports")]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports()
        {
            try
            {
                // Fetch reports from the database
                var reports = await _context.Reports.ToListAsync();

                if (reports == null || reports.Count == 0)
                {
                    return NotFound("No reports found.");
                }

                return Ok(reports); // Return the reports as JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}