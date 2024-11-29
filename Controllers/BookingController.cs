using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BookingController : ControllerBase 
    {
        private readonly AutoFleetDbContext _context;

        public BookingController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        [HttpGet("rental-status/{renterId}")]
        public async Task<IActionResult> GetRentalStatus(int renterId, [FromQuery] string status)
        {
            var query = _context.RentedVehicles.Where(rv => rv.renter_id == renterId);

            switch (status.ToLower())
            {
                case "upcoming":
                    query = query.Where(rv => rv.pickup_date > DateTime.Now || rv.rent_status == "Upcoming");
                    break;
                case "completed":
                    query = query.Where(rv => rv.rent_status == "Completed");
                    break;
                case "canceled":
                    query = query.Where(rv => rv.rent_status == "Canceled");
                    break;
                default: 
                    return BadRequest(new { Message = "Invalid status." });
            }

            var rentals = await query
                .OrderBy(rv => rv.pickup_date) // Sort by pickup date for example
                .Select(rv => new
                {
                    rv.car_model,
                    PickupDate = rv.pickup_date.ToString("yyyy-MM-dd"),
                    PickupTime = rv.pickup_time is TimeSpan
                        ? ((TimeSpan)rv.pickup_time).ToString(@"hh\:mm\:ss")
                        : ((TimeSpan)rv.pickup_time).ToString("HH:mm:ss"),
                    Status = rv.rent_status,
                    VehicleId = rv.vehicle_id,
                })
                .ToListAsync();

            return Ok(rentals);
        }



    }
}