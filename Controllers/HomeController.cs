using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class HomeController : ControllerBase 
    {
        private readonly AutoFleetDbContext _context;

        public HomeController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("recent-trips/{renterId}")]
        public async Task<IActionResult> GetRecentTrips(int renterId)
        {
            var recentTrips = await _context.RentedVehicles
                .Where(rv => rv.renter_id == renterId)
                .OrderByDescending(rv => rv.pickup_date) // Sort by the most recent trips
                .Select(rv => new
                {
                    rv.car_model,
                    PickupDate = rv.pickup_date.ToString("yyyy-MM-dd"),
                    PickupTime = rv.pickup_time is TimeSpan 
                        ? ((TimeSpan)rv.pickup_time).ToString(@"hh\:mm\:ss") 
                        : ((TimeSpan)rv.pickup_time).ToString("HH:mm:ss")
                })
                .ToListAsync();

            return Ok(recentTrips);
        }

        



    }
}