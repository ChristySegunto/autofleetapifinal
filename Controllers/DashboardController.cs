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
                    .Take(5) // Limit the number of results to 5 (optional)
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

                return Ok(recentBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }

}