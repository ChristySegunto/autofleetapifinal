using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly AutoFleetDbContext _context;

    public ReportController(AutoFleetDbContext context)
    {
        _context = context;
    }

    // POST: api/Report/list
    [HttpGet("list")]
    public IActionResult GetReportList()
    {
        var reportList = _context.Reports.Select(r => new
        {
            r.report_id,
            r.renter_id,
            r.nature_of_issue,
            r.emergency,
            date = r.date.ToString("yyyy-MM-dd"),
            time = r.time.ToString("hh:mm:ss"),
            r.note
        }).ToList();

        return Ok(reportList);
    }

    [HttpPost("addReport")]
    public async Task<IActionResult> AddReport([FromBody] Report report)
    {
        if (report == null)
        {
            return BadRequest("Report data is null.");
        }

        if (report.emergency == "true")
        {
            report.emergency = "y";
        }
        else if (report.emergency == "false")
        {
            report.emergency = "n";
        }

        // Ensure that all necessary fields are provided
        if (string.IsNullOrEmpty(report.nature_of_issue) ||
            string.IsNullOrEmpty(report.note) ||
            report.date == default ||
            report.time == default)
        {
            return BadRequest("All fields are required.");
        }

        // // Parse the time if it's a valid string format
        // if (string.IsNullOrEmpty(report.time.ToString()))
        // {
        //     report.time = TimeSpan.Parse("00:00:00"); // Or handle this according to your logic
        // }

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
            return StatusCode(StatusCodes.Status500InternalServerError, "Error saving report data: " + ex.Message);
        }
    }



}
