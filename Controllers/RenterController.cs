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
                // renter_id_photo_1 = r.renter_id_photo_1 != null 
                //     ? Convert.ToBase64String(r.renter_id_photo_1) // Convert binary to Base64 string
                //     : null

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

            // Ensure that all necessary fields are provided
            if (string.IsNullOrEmpty(renter.renter_fname) ||
                string.IsNullOrEmpty(renter.renter_mname) ||
                string.IsNullOrEmpty(renter.renter_lname) ||
                renter.renter_birthday == default ||
                string.IsNullOrEmpty(renter.renter_contact_num) ||
                string.IsNullOrEmpty(renter.renter_email) ||
                string.IsNullOrEmpty(renter.renter_emergency_contact) ||
                string.IsNullOrEmpty(renter.renter_address)
                // renter.renter_id_photo_1 == null
                )
            {
                return BadRequest("All fields are required.");
            }


             _context.Renters.Add(renter);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Now link the renter with the user by updating the user_id
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == renter.renter_email);
                if (user != null)
                {
                    renter.user_id = user.user_id;
                    await _context.SaveChangesAsync();
                }

                // Return the newly created renter record as a response
                return CreatedAtAction(nameof(GetRenterList), new { id = renter.renter_id }, renter);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving renter data: " + ex.Message);
            }
        }

        [HttpPost("addAcc")]
        public async Task<IActionResult> AddAccount([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("user data is null.");
            }

            if (string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password) )
            {
                return BadRequest("All fields are required.");
            }

            // Add the renter data to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRenterList), new { id = user.user_id }, user);

        }

    }
}