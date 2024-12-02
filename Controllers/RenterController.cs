using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RenterController : ControllerBase
    {
        private readonly AutoFleetDbContext _context;

        public RenterController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("list")]
        public IActionResult GetRenterList()
        {
            var renterList = _context.Renters.Select(r => new
            {
                r.renter_id,
                r.renter_fname,
                r.renter_mname,
                r.renter_lname,
                birthDay = r.renter_birthday.ToString("yyyy-MM-dd"),
                r.renter_contact_num,
                r.renter_email,
                r.renter_emergency_contact,
                r.renter_address,
                r.renter_id_photo_1

            }).ToList();

            return Ok(renterList);
        }

         [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmailUniqueness([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required");
        }

        // Check if email already exists in the database
        var existingRenter = await _context.Renters
            .FirstOrDefaultAsync(r => r.renter_email.ToLower() == email.ToLower());

        return Ok(existingRenter == null);
    }

        [HttpPost("addRenter")]
        public async Task<IActionResult> AddRenter([FromBody] Renter renter)
        {
            if (renter == null)
            {
                return BadRequest("Renter data is null.");
            }

            // Ensure that all necessary field are provided
            if (string.IsNullOrEmpty(renter.renter_fname) ||
                string.IsNullOrEmpty(renter.renter_mname) ||
                string.IsNullOrEmpty(renter.renter_lname) ||
                renter.renter_birthday == default ||
                string.IsNullOrEmpty(renter.renter_contact_num) ||
                string.IsNullOrEmpty(renter.renter_email) ||
                string.IsNullOrEmpty(renter.renter_emergency_contact) ||
                string.IsNullOrEmpty(renter.renter_address) ||
                string.IsNullOrEmpty(renter.renter_id_photo_1)||
                renter.user_id == 0
                )
            {
                return BadRequest("All fields are required.");
            }

            // Add the renter data to the database
            _context.Renters.Add(renter);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return the newly created renter record as a response
                return CreatedAtAction(nameof(GetRenterList), new { id = renter.renter_id }, renter);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving renter data: " + ex.Message);
            }
        }

    }
}