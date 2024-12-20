using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly AutoFleetDbContext _context;

    public VehicleController(AutoFleetDbContext context)
    {
        _context = context;
    }

    // GET: api/Vehicle/list
    [HttpGet("list")]
    public IActionResult GetVehicleList()
    {
        var vehicleList = _context.Vehicles.Select(v => new
        {
            v.vehicle_id,
            v.car_manufacturer,
            v.car_model,
            v.plate_number,
            v.manufacture_year,
            v.vehicle_color,
            v.fuel_type,
            v.transmission_type,
            v.seating_capacity,
            v.vehicle_category,
            v.total_mileage,
            v.total_fuel_consumption,
            v.distance_traveled,
            v.vehicle_status,
            created_at = v.created_at.ToString("yyyy-MM-dd"),
            updated_at = v.updated_at.ToString("yyyy-MM-dd")
        }).ToList();

        return Ok(vehicleList);
    }

    // GET: api/Vehicle/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVehicleById(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        return Ok(new
        {
            vehicle.vehicle_id,
            vehicle.car_manufacturer,
            vehicle.car_model,
            vehicle.plate_number,
            vehicle.manufacture_year,
            vehicle.vehicle_color,
            vehicle.fuel_type,
            vehicle.transmission_type,
            vehicle.seating_capacity,
            vehicle.vehicle_category,
            vehicle.total_mileage,
            vehicle.total_fuel_consumption,
            vehicle.distance_traveled,
            vehicle.vehicle_status,
            created_at = vehicle.created_at.ToString("yyyy-MM-dd"),
            updated_at = vehicle.updated_at.ToString("yyyy-MM-dd")
        });
    }

    // POST: api/Vehicle
    [HttpPost]
    public async Task<IActionResult> AddVehicle([FromBody] Vehicle vehicle)
    {
        if (vehicle == null)
        {
            return BadRequest("Vehicle data is null.");
        }

        // Validate required fields
        if (string.IsNullOrEmpty(vehicle.car_manufacturer) ||
            string.IsNullOrEmpty(vehicle.car_model) ||
            string.IsNullOrEmpty(vehicle.plate_number) ||
            vehicle.manufacture_year == 0)
        {
            return BadRequest("Required fields are missing.");
        }

        vehicle.created_at = DateTime.UtcNow;
        vehicle.updated_at = DateTime.UtcNow;

        _context.Vehicles.Add(vehicle);

        try
        {
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVehicleById), new { id = vehicle.vehicle_id }, vehicle);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error saving vehicle data: " + ex.Message);
        }
    }

    // PUT: api/Vehicle/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, [FromBody] Vehicle vehicle)
    {
        if (vehicle == null)
        {
            return BadRequest("Vehicle data is null.");
        }

        // Validate required fields
        if (string.IsNullOrEmpty(vehicle.car_manufacturer) ||
            string.IsNullOrEmpty(vehicle.car_model) ||
            string.IsNullOrEmpty(vehicle.plate_number) ||  // Ensure plate_number is not empty
            vehicle.manufacture_year == 0)
        {
            return BadRequest("Required fields are missing.");
        }

        // Check if the vehicle exists
        var existingVehicle = await _context.Vehicles.FindAsync(id);
        if (existingVehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        // Check if the new plate_number already exists for another vehicle
        var existingPlate = await _context.Vehicles
            .Where(v => v.plate_number == vehicle.plate_number && v.vehicle_id != id)
            .FirstOrDefaultAsync();

        if (existingPlate != null)
        {
            return BadRequest("The plate number is already in use by another vehicle.");
        }

        // Update the vehicle
        existingVehicle.car_manufacturer = vehicle.car_manufacturer;
        existingVehicle.car_model = vehicle.car_model;
        existingVehicle.manufacture_year = vehicle.manufacture_year;
        existingVehicle.vehicle_color = vehicle.vehicle_color;
        existingVehicle.fuel_type = vehicle.fuel_type;
        existingVehicle.transmission_type = vehicle.transmission_type;
        existingVehicle.seating_capacity = vehicle.seating_capacity;
        existingVehicle.vehicle_category = vehicle.vehicle_category;
        existingVehicle.total_mileage = vehicle.total_mileage;
        existingVehicle.total_fuel_consumption = vehicle.total_fuel_consumption;
        existingVehicle.distance_traveled = vehicle.distance_traveled;
        existingVehicle.vehicle_status = vehicle.vehicle_status;
        existingVehicle.plate_number = vehicle.plate_number;  // Update the plate number
        existingVehicle.updated_at = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingVehicle);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating vehicle data: " + ex.Message);
        }
    }

    // PUT: api/Vehicle/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateVehicleStatus(int id, [FromBody] string status)
    {
        if (string.IsNullOrEmpty(status))
        {
            return BadRequest("Status cannot be empty.");
        }

        // Check if the vehicle exists
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        // Update the vehicle status
        vehicle.vehicle_status = status;
        vehicle.updated_at = DateTime.UtcNow;

        // Log the updated vehicle to check the changes
        Console.WriteLine($"Updated vehicle: ID = {vehicle.vehicle_id}, Status = {vehicle.vehicle_status}, Updated At = {vehicle.updated_at}");

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Vehicle status updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating vehicle status: " + ex.Message);
        }
    }


    // DELETE: api/Vehicle/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        _context.Vehicles.Remove(vehicle);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Vehicle deleted successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting vehicle: " + ex.Message);
        }
    }
}
