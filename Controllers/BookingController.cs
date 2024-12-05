using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    // Define the route for the controller (this will be the base URL for API requests)
    [Route("api/[controller]")]
    [ApiController]
    
    public class BookingController : ControllerBase 
    {
        private readonly AutoFleetDbContext _context;

        // Constructor injection to get the database context for querying the database
        public BookingController(AutoFleetDbContext context)
        {
            // If the context is null, an exception will be thrown
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Booking/rental-status/{renterId}
        // API endpoint to get the rental status for a specific renter based on their renterId and status query
        [HttpGet("rental-status/{renterId}")]
        public async Task<IActionResult> GetRentalStatus(int renterId, [FromQuery] string status)
        {
            // Start by querying the RentedVehicles table where the renter_id matches the provided renterId
            var query = _context.RentedVehicles.Where(rv => rv.renter_id == renterId);

            // Filter the query based on the 'status', handling different cases
            switch (status.ToLower())
            {
                case "upcoming":
                    // Only select vehicles with an 'Upcoming' rental status
                    query = query.Where(rv => rv.rent_status == "Upcoming");
                    break;
                case "completed":
                    // Only select vehicles with a 'Completed' rental status
                    query = query.Where(rv => rv.rent_status == "Completed");
                    break;
                case "canceled":
                    // Only select vehicles with a 'Canceled' rental status
                    query = query.Where(rv => rv.rent_status == "Canceled");
                    break;
                default: 
                    // If the status is not recognized, return a BadRequest response with a message
                    return BadRequest(new { Message = "Invalid status." });
            }

            // Execute the query asynchronously, sorting by the pickup date (ascending) and selecting specific fields
            var rentals = await query
                .OrderBy(rv => rv.pickup_date) // Sort by pickup date for example
                .Select(rv => new
                {
                    rv.car_model, // Car model of the rented vehicle
                    PickupDate = rv.pickup_date.ToString("yyyy-MM-dd"), // Format the pickup date
                    PickupTime = rv.pickup_time is TimeSpan
                        ? ((TimeSpan)rv.pickup_time).ToString(@"hh\:mm\:ss") // Format pickup time if it's a valid TimeSpan
                        : ((TimeSpan)rv.pickup_time).ToString("HH:mm:ss"), // Otherwise, format as a 24-hour time
                    Status = rv.rent_status, // Current rental status
                    VehicleId = rv.vehicle_id, // Vehicle identifier
                    RentedVehicleId = rv.rented_vehicle_id, // Unique identifier for the rented vehicle record
                })
                .ToListAsync(); // Convert the query result to a list asynchronously

            // Return the rentals in the response as an Ok result (HTTP 200)
            return Ok(rentals);
        }



    }
}