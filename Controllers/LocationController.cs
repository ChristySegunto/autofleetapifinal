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

        // Start a new trip: Create a new CarUpdate record
        [HttpPost("start-trip")]
        public async Task<IActionResult> StartTrip([FromBody] CarUpdate carUpdate)
        {
            if (carUpdate == null)
            {
                return BadRequest(new { Message = "Invalid CarUpdate data." });
            }

            // Validate that vehicle_id and renter_id exist in their respective tables
            var vehicle = await _context.Vehicles.SingleOrDefaultAsync(v => v.vehicle_id == carUpdate.vehicle_id);
            if (vehicle == null)
            {
                return BadRequest(new { Message = "Vehicle not found." });
            }

            var renter = await _context.Renters.SingleOrDefaultAsync(r => r.renter_id == carUpdate.renter_id);
            if (renter == null)
            {
                return BadRequest(new { Message = "Renter not found." });
            }

            // Validate that the rented_vehicle_id exists in the RentedVehicle table
            var rentedVehicle = await _context.RentedVehicles.SingleOrDefaultAsync(rv => rv.rented_vehicle_id == carUpdate.rented_vehicle_id);
            if (rentedVehicle == null)
            {
                return BadRequest(new { Message = "Rented vehicle not found." });
            }

            // Set the current timestamp and status
            carUpdate.last_update = DateTime.UtcNow;
            carUpdate.carupdate_status = "Ongoing";

            // Add the new CarUpdate record to the database
            _context.CarUpdates.Add(carUpdate);
            await _context.SaveChangesAsync(); // Save to generate the auto-incremented carupdate_id

            // Return the response with the generated carupdate_id
            return Ok(new { Message = "Trip started successfully", carUpdate });
        }


        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] CarUpdate carUpdate)
        {
            // Validate that vehicle exists
            var vehicle = await _context.Vehicles.SingleOrDefaultAsync(v => v.vehicle_id == carUpdate.vehicle_id);
            if (vehicle == null)
            {
                return BadRequest(new { Message = "Vehicle not found." });
            }

            // Validate that renter exists
            var renter = await _context.Renters.SingleOrDefaultAsync(r => r.renter_id == carUpdate.renter_id);
            if (renter == null)
            {
                return BadRequest(new { Message = "Renter not found." });
            }

            // Validate rented vehicle exists
            var rentedVehicle = await _context.RentedVehicles.SingleOrDefaultAsync(rv => rv.rented_vehicle_id == carUpdate.rented_vehicle_id);
            if (rentedVehicle == null)
            {
                return BadRequest(new { Message = "Rented vehicle not found." });
            }

            // Create a new CarUpdate record (i.e., insert a new row in the database)
            carUpdate.last_update = DateTime.UtcNow;  // Set the current timestamp for the update
            carUpdate.carupdate_status = "Ongoing"; // Ensure the trip is ongoing

            // Add the new CarUpdate record to the database
            await _context.CarUpdates.AddAsync(carUpdate);
            
            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the new CarUpdate data
            return Ok(new { Message = "Location updated successfully", carUpdate });
        }

        // End the trip: Mark the trip as completed and stop updates
        [HttpPut("end-trip/{rented_vehicle_id}")]
        public async Task<IActionResult> EndTrip([FromRoute] int rented_vehicle_id)
        {
            // Retrieve all updates with the same rented_vehicle_id
            var existingUpdates = await _context.CarUpdates
                                                .Where(u => u.rented_vehicle_id == rented_vehicle_id)
                                                .ToListAsync();

            // If no records are found
            if (!existingUpdates.Any())
            {
                return NotFound(new { Message = "No car updates found for the given rented vehicle ID." });
            }

            // Update the carupdate_status for all matching records to "Completed"
            foreach (var update in existingUpdates)
            {
                update.carupdate_status = "Completed"; // Mark the trip as completed
                update.last_update = DateTime.UtcNow;  // Update the last update timestamp
            }

            // Save all the changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { Message = "All trips for the rented vehicle have been completed.", existingUpdates });
        }


    }


    
}