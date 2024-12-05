using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace autofleetapi.Controllers
{
    // Define the route for the controller
    [Route("api/[controller]")]
    [ApiController]
    
    public class DashboardController : ControllerBase 
    {

        private readonly AutoFleetDbContext _context;

        // Constructor that accepts AutoFleetDbContext for querying the database
        public DashboardController(AutoFleetDbContext context)
        {
            // Throw an exception if the context is null
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Dashboard/get-admin-details
        // Fetch details of the logged-in admin based on userId
        [HttpGet("get-admin-details")]
        public async Task<IActionResult> GetLoggedInAdmin([FromQuery] int userId)
        {
            try
            {

                // Check if the userId is valid
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                // Directly query the Admin table based on user_id
                var admin = await _context.Admins
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.user_id == userId);

                // Return NotFound if admin or associated user doesn't exist
                if (admin == null || admin.User == null)
                {
                    return NotFound("Admin or associated user not found.");
                }

                // Return the admin details in the response
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
            { // Log the exception and return a 500 status code with a message
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Dashboard/vehicleslist
        // Fetch the list of all vehicles in the database
        [HttpGet("vehicleslist")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            try
            {
                // Fetch all vehicles from the database
                var vehicles = await _context.Vehicles.ToListAsync();
                return Ok(vehicles); // Return the vehicles list
            }

            catch (Exception ex)
            {
                // Handle errors and return 500 status code
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/Dashboard/Count
        // Fetch the total number of vehicles in the system
        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetTotalVehiclesCount()
        {
            try
            {
                // Get the count of total vehicles
                int totalCount = await _context.Vehicles.CountAsync();
                return Ok(totalCount); // Return the count of vehicles
            }
            catch (Exception ex)
            {
                // Handle errors and return 500 status code
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/Dashboard/StatusCount
        // Fetch the count of vehicles by its status 
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

                // Return the counts for each status in an anonymous object
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
                // Handle errors and return 500 status code
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/Vehicles/CategoryCount
        // Fetch the count of vehicles by its category
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

                // Return the counts for each category
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
                // Handle errors and return 500 status code
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/Dashboard/RecentBookings
        // Fetch the recent bookings from the RentedVehicles table
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

                // If no bookings found, return NotFound
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

                return Ok(recentBookings); // Return the list of recent bookings
            }
            catch (Exception ex)
            {
                // Handle errors and return 500 status code
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/Dashboard/get-today-schedules
        // Fetch rental and maintenance schedules for today and yesterday
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

        // GET: api/Dashboard/Reports
        // Fetch all reports from the database
        [HttpGet("Reports")]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports()
        {
            try
            {
                // Fetch reports from the database
                var reports = await _context.Reports.ToListAsync();

                // If no reports found, return NotFound
                if (reports == null || reports.Count == 0)
                {
                    return NotFound("No reports found.");
                }

                return Ok(reports); // Return the reports as JSON
            }
            catch (Exception ex)
            {
                // Handle errors and return 500 status code
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}