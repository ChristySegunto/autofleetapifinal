using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly AutoFleetDbContext _context;

    // Constructor to initialize the controller with the database context
    public ReportController(AutoFleetDbContext context)
    {
        _context = context; // Assign the database context to interact with Reports table
    }

    // POST: api/Report/list
    // Retrieves a list of reports with relevant details like renter ID, nature of issue, and timestamps
    [HttpGet("list")]
    public IActionResult GetReportList()
    {
        // Query the Reports table and select specific fields to return
        var reportList = _context.Reports.Select(r => new
        {
            r.report_id,
            r.renter_id,
            r.nature_of_issue,
            r.emergency,
            date = r.date.ToString("yyyy-MM-dd"), // Format date to string
            time = r.time.ToString("hh:mm:ss"), // Format time to string
            r.note
        }).ToList();

        // Return the list of reports as a successful response
        return Ok(reportList);
    }

    // POST: api/Report/addReport
    // Adds a new report to the system
    [HttpPost("addReport")]
    public async Task<IActionResult> AddReport([FromBody] Report report)
    {
        // Check if the report data is null
        if (report == null)
        {
            return BadRequest("Report data is null.");
        }

        // Normalize the "emergency" field from string "true"/"false" to "y"/"n"
        if (report.emergency == "true")
        {
            report.emergency = "y"; // Emergency is marked as "yes"
        }
        else if (report.emergency == "false")
        {
            report.emergency = "n"; // Emergency is marked as "no"
        }

        // Ensure that all necessary fields are provided
        if (string.IsNullOrEmpty(report.nature_of_issue) ||
            string.IsNullOrEmpty(report.note) ||
            report.date == default ||
            report.time == default)
        {
            return BadRequest("All fields are required."); // Return an error if any field is missing
        }

        // Validate that the renter exists in the database
        var renter = await _context.Renters.FindAsync(report.renter_id);
        if (renter == null)
        {
            return BadRequest("Renter not found.");
        }

        // Add the report data to the database
        _context.Reports.Add(report);

        try
        {
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the newly created report record as a response
            return CreatedAtAction(nameof(GetReportList), new { id = report.report_id }, report);
        }
        catch (Exception ex)
        {
            // If an error occurs while saving, return a server error response
            return StatusCode(StatusCodes.Status500InternalServerError, "Error saving report data: " + ex.Message);
        }
    }

}
