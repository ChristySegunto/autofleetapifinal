using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    // Define the route for the controller
    [Route("api/[controller]")]
    [ApiController]
    
    public class HomeController : ControllerBase 
    {
        private readonly AutoFleetDbContext _context;

        // Constructor that accepts AutoFleetDbContext for querying the database
        public HomeController(AutoFleetDbContext context)
        {
            // Throw an exception if the context is null
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Home/recent-trips/{renterId}
        // Fetch the recent trips for a specific renter by renterId
        [HttpGet("recent-trips/{renterId}")]
        public async Task<IActionResult> GetRecentTrips(int renterId)
        {
            // Query the RentedVehicles table to get recent trips for the specified renter, ordered by pickup date in descending order
            var recentTrips = await _context.RentedVehicles
                .Where(rv => rv.renter_id == renterId) // Filter by renterId
                .OrderByDescending(rv => rv.pickup_date) // Sort by the most recent trips
                .Select(rv => new
                {
                    rv.car_model,
                    PickupDate = rv.pickup_date.ToString("yyyy-MM-dd"), // Format the pickup date
                    PickupTime = rv.pickup_time is TimeSpan 
                        ? ((TimeSpan)rv.pickup_time).ToString(@"hh\:mm\:ss") // Format pickup time as HH:mm:ss if it is a TimeSpan
                        : ((TimeSpan)rv.pickup_time).ToString("HH:mm:ss") // Format pickup time as HH:mm:ss otherwise
                })
                .ToListAsync(); // Execute the query asynchronously

            // Return the list of recent trips as a successful response
            return Ok(recentTrips);
        }

    }
}