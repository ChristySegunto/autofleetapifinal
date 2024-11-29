using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class LocationController : ControllerBase 
    {
        private readonly AutoFleetDbContext _context;

        public LocationController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //for starting the trip
        [HttpPost("start-trip")]
        public IActionResult StartTrip([FromBody] CarUpdate carUpdate)
        {
            // Check if the carUpdate object is null
            if (carUpdate == null)
            {
                return BadRequest(new { Message = "Invalid data." });
            }

            // Validate that vehicle_id exists in the Vehicle table
            var vehicle = _context.Vehicles.SingleOrDefault(v => v.vehicle_id == carUpdate.vehicle_id);
            if (vehicle == null)
            {
                return BadRequest(new { Message = "Invalid vehicle. Vehicle ID not found." });
            }

            // Ensure that the carupdate_status is provided
            if (string.IsNullOrWhiteSpace(carUpdate.carupdate_status))
            {
                return BadRequest(new { Message = "Car update status is required." });
            }

            // Additional checks can be added here as needed for speed, fuel consumption, etc.

            // Add the carUpdate to the database and save changes
            _context.CarUpdates.Add(carUpdate);
            _context.SaveChanges();

            return Ok(new { Message = "Trip started successfully", carUpdate });
        }

        // Controller method for updating ongoing trip details
        [HttpPut("update-trip/{carupdate_id}")]
        public IActionResult UpdateTrip(int carupdate_id, [FromBody] CarUpdate carUpdate)
        {
            if (carUpdate == null)
            {
                return BadRequest(new { Message = "Invalid payload." });
            }


            var existingUpdate = _context.CarUpdates.SingleOrDefault(u => u.carupdate_id == carupdate_id);
            if (existingUpdate == null)
            {
                return NotFound(new { Message = "Car update not found." });
            }


            // Update relevant fields
            existingUpdate.location_latitude = carUpdate.location_latitude;
            existingUpdate.location_longitude = carUpdate.location_longitude;
            existingUpdate.speed = carUpdate.speed;
            existingUpdate.total_fuel_consumption = carUpdate.total_fuel_consumption;
            existingUpdate.total_distance_travelled = carUpdate.total_distance_travelled;
            existingUpdate.last_update = carUpdate.last_update;

            _context.SaveChanges();
            
            return Ok(new { Message = "Trip updated successfully", existingUpdate });
        }

        // Controller method for ending the trip
        [HttpPut("end-trip/{carupdate_id}")]
        public IActionResult EndTrip(int carupdate_id)
        {
            var existingUpdate = _context.CarUpdates.SingleOrDefault(u => u.carupdate_id == carupdate_id);
            if (existingUpdate == null)
            {
                return NotFound(new { Message = "Car update not found." });
            }

            existingUpdate.carupdate_status = "Completed"; // Mark the trip as completed
            _context.SaveChanges();

            return Ok(new { Message = "Trip completed successfully", existingUpdate });
        }



    }
}