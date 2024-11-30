using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentedVehicleController : ControllerBase
    {
        private readonly AutoFleetDbContext _context;

        public RentedVehicleController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentedVehicle>>> GetRentedVehicles()
        {
            try
            {
                var rentedVehicles = await _context.RentedVehicles
                    .Include(r => r.Renter)
                    .Include(v => v.Vehicle)
                    .ToListAsync();
                return Ok(rentedVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving rented vehicles: " + ex.Message);
            }
        }

[HttpPost("add")]
public async Task<IActionResult> AddRentedVehicle([FromBody] RentedVehicle rentedVehicle)
{
    if (rentedVehicle == null)
    {
        return BadRequest("Rented vehicle data is null.");
    }

    // Log the received data to check for any missing/incorrect fields
    Console.WriteLine($"Received rented vehicle data: {rentedVehicle.renter_fname}, {rentedVehicle.renter_lname}, {rentedVehicle.pickup_date}, {rentedVehicle.dropoff_date}");

    // Validate fields to ensure all required data is provided
    if (string.IsNullOrEmpty(rentedVehicle.renter_fname) ||
        string.IsNullOrEmpty(rentedVehicle.renter_lname) ||
        rentedVehicle.pickup_date == default ||
        rentedVehicle.dropoff_date == default ||
        rentedVehicle.pickup_time == default ||
        rentedVehicle.dropoff_time == default ||
        string.IsNullOrEmpty(rentedVehicle.car_manufacturer) ||
        string.IsNullOrEmpty(rentedVehicle.car_model) ||
        string.IsNullOrEmpty(rentedVehicle.plate_number) ||
        string.IsNullOrEmpty(rentedVehicle.rent_status))
    {
        // Log the validation failure details
        Console.WriteLine("Validation failed for rented vehicle data.");
        return BadRequest("All fields are required.");
    }

    // Set default values for created_at and updated_at if not provided
    rentedVehicle.created_at = DateTime.UtcNow;
    rentedVehicle.updated_at = DateTime.UtcNow;

    _context.RentedVehicles.Add(rentedVehicle);

    try
    {
        // Save changes to the database
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRentedVehicles), new { id = rentedVehicle.rented_vehicle_id }, rentedVehicle);
    }
    catch (Exception ex)
    {
        // Log the error if there is an exception during database save
        Console.WriteLine($"Error saving rented vehicle: {ex.Message}");
        return StatusCode(StatusCodes.Status500InternalServerError, "Error saving rented vehicle: " + ex.Message);
    }
}



    }
}
