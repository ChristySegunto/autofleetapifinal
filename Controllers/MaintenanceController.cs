using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    private readonly AutoFleetDbContext _context;

    // Constructor to inject AutoFleetDbContext into the controller
    public MaintenanceController(AutoFleetDbContext context)
    {
        _context = context;
    }

    // GET: api/Maintenance/list
    // Fetches a list of all maintenance records with relevant details
    [HttpGet("list")]
    public IActionResult GetMaintenanceList()
    {
        // Retrieves a list of maintenance records and selects relevant fields for the response
        var maintenanceList = _context.Maintenances.Select(m => new
        {
            m.maintenance_id,
            m.plate_num,
            m.car_model,
            m.maintenance_type,
            // Convert the dates to "yyyy-MM-dd" format for better readability
            dueDate = m.maintenance_due_date.ToString("yyyy-MM-dd"),
            nextDueDate = m.maintenance_next_due_date.ToString("yyyy-MM-dd"),
            m.maintenance_status
        }).ToList();

        // Returns the list as an OK response
        return Ok(maintenanceList);
    }

    // GET: api/Maintenance/plateNumbers
    // Fetches all unique plate numbers of vehicles for maintenance
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

    // POST: api/Maintenance/addMaintenance
    // Adds a new maintenance record to the system
    [HttpPost("addMaintenance")]
    public async Task<IActionResult> AddMaintenance([FromBody] Maintenance maintenance)
    {
        if (maintenance == null)
        {
            return BadRequest("Maintenance data is null.");  // If the maintenance object is null, return a bad request
        }

        // Ensure that all necessary fields are provided
        if (string.IsNullOrEmpty(maintenance.plate_num) ||
            string.IsNullOrEmpty(maintenance.car_model) ||
            string.IsNullOrEmpty(maintenance.maintenance_type) ||
            maintenance.maintenance_due_date == default ||
            maintenance.maintenance_next_due_date == default ||
            maintenance.vehicle_id == 0)
        {
            return BadRequest("All fields are required."); // If any required field is missing, return a bad request
        }

        // Validate that the vehicle exists in the database
        var vehicle = await _context.Vehicles.FindAsync(maintenance.vehicle_id);
        if (vehicle == null)
        {
            return BadRequest("Vehicle not found."); // Return error if vehicle is not found in the database
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
            // Return an error response if saving the maintenance record fails
            return StatusCode(StatusCodes.Status500InternalServerError, "Error saving maintenance data: " + ex.Message);
        }
    }

    // PUT: api/Maintenance/updateStatus/{id}
    // Updates the status of a specific maintenance record
    [HttpPut("updateStatus/{id}")]
    public IActionResult UpdateStatus(int id, [FromBody] Maintenance maintenance)
    {
        // Find the existing maintenance record by ID
        var existingMaintenance = _context.Maintenances.Find(id);
        if (existingMaintenance == null)
            return NotFound(); // Return 404 if maintenance record is not found

        // Ensure maintenance status is provided in the request body
        if (string.IsNullOrEmpty(maintenance.maintenance_status))
            return BadRequest("Maintenance status is required.");

        // Update the maintenance status and timestamp
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
            vehicle.vehicle_status = "Under Maintenance"; // Update vehicle status when maintenance is ongoing
        }
        else if (maintenanceStatus == "completed")
        {
            vehicle.vehicle_status = "Available"; // Update vehicle status when maintenance is completed
        }

        vehicle.updated_at = DateTime.UtcNow; // Update vehicle timestamp

        // Save the changes to the database
        _context.SaveChanges();
        return Ok(existingMaintenance); // Return the updated maintenance record as response
    }


    // DELETE: api/Maintenance/delete/{maintenance_id}
    // Deletes a maintenance record from the database
    [HttpDelete("delete/{maintenance_id}")]
    public async Task<IActionResult> DeleteMaintenance(int maintenance_id)
    {
        // Find the maintenance record by ID
        var maintenance = await _context.Maintenances.FindAsync(maintenance_id);

        if (maintenance == null)
        {
            return NotFound($"Maintenance record with ID {maintenance_id} not found."); // Return 404 if the record is not found
        }

        // Remove the maintenance record from the database
        _context.Maintenances.Remove(maintenance);

        try
        {
            // Save changes to the database and return success response
            await _context.SaveChangesAsync();
            return Ok(new { message = "Maintenance record deleted successfully." });
        }
        catch (Exception ex)
        {
            // Return an error response if deletion fails
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting maintenance record: {ex.Message}");
        }
    }







}
