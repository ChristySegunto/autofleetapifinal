using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    private readonly AutoFleetDbContext _context;

    public MaintenanceController(AutoFleetDbContext context)
    {
        _context = context;
    }

    // POST: api/Maintenance/list
    [HttpGet("list")]
    public IActionResult GetMaintenanceList()
    {
        var maintenanceList = _context.Maintenances.Select(m => new
        {
            m.maintenance_id,
            m.plate_num,
            m.car_model,
            m.maintenance_type,
            dueDate = m.maintenance_due_date.ToString("yyyy-MM-dd"),
            nextDueDate = m.maintenance_next_due_date.ToString("yyyy-MM-dd"),
            m.maintenance_status
        }).ToList();

        return Ok(maintenanceList);
    }

    [HttpGet("plateNumbers")]
    public async Task<IActionResult> GetPlateNumbers()
    {
        try
        {
            // Fetch unique plate numbers from the database
            var plateNumbers = await _context.Vehicles
                .Select(v => new { v.plate_number, v.car_model, v.vehicle_id })
                .ToListAsync();

            // Return the list of plate numbers as JSON
            return Ok(plateNumbers);
        }
        catch (Exception ex)
        {
            // Return an error response if something goes wrong
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("addMaintenance")]
    public async Task<IActionResult> AddMaintenance([FromBody] Maintenance maintenance)
    {
        if (maintenance == null)
        {
            return BadRequest("Maintenance data is null.");
        }

        // Ensure that all necessary fields are provided
        if (string.IsNullOrEmpty(maintenance.plate_num) ||
            string.IsNullOrEmpty(maintenance.car_model) ||
            string.IsNullOrEmpty(maintenance.maintenance_type) ||
            maintenance.maintenance_due_date == default ||
            maintenance.maintenance_next_due_date == default ||
            maintenance.vehicle_id == 0)
        {
            return BadRequest("All fields are required.");
        }

        // Validate that the vehicle exists in the database
        var vehicle = await _context.Vehicles.FindAsync(maintenance.vehicle_id);
        if (vehicle == null)
        {
            return BadRequest("Vehicle not found.");
        }

        // Add the maintenance data to the database
        _context.Maintenances.Add(maintenance);

        try
        {
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the newly created maintenance record as a response
            return CreatedAtAction(nameof(GetMaintenanceList), new { id = maintenance.maintenance_id }, maintenance);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error saving maintenance data: " + ex.Message);
        }
    }

    [HttpPut("updateStatus/{id}")]
    public IActionResult UpdateStatus(int id, [FromBody] Maintenance maintenance)
    {
        var existingMaintenance = _context.Maintenances.Find(id);
        if (existingMaintenance == null)
            return NotFound();

        if (string.IsNullOrEmpty(maintenance.maintenance_status))
            return BadRequest("Maintenance status is required.");

        existingMaintenance.maintenance_status = maintenance.maintenance_status;
        existingMaintenance.updated_at = DateTime.UtcNow;

        // Fetch the related vehicle
        var vehicle = _context.Vehicles.Find(existingMaintenance.vehicle_id);
        if (vehicle == null)
        {
            return NotFound($"No vehicle found for vehicle_id: {existingMaintenance.vehicle_id}");
        }

        // Normalize and update vehicle status based on maintenance status
        var maintenanceStatus = maintenance.maintenance_status.ToLower();
        if (maintenanceStatus == "under maintenance")
        {
            vehicle.vehicle_status = "Under Maintenance";
        }
        else if (maintenanceStatus == "completed")
        {
            vehicle.vehicle_status = "Available";
        }

        vehicle.updated_at = DateTime.UtcNow; // Update vehicle timestamp

        
        _context.SaveChanges();
        return Ok(existingMaintenance);
    }






}
