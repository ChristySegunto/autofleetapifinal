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

        // Constructor to initialize the controller with the database context
        public RenterController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Renter/list
        // Retrieves a list of renters with relevant information
        [HttpGet("list")]
        public IActionResult GetRenterList()
        {
            // Query the Renters table to select specific details of each renter
            var renterList = _context.Renters.Select(r => new
            {
                r.renter_id,
                r.renter_fname,
                r.renter_mname,
                r.renter_lname,
                birthDay = r.renter_birthday.ToString("yyyy-MM-dd"), // Format birthday to a string
                r.renter_contact_num,
                r.renter_email,
                r.renter_emergency_contact,
                r.renter_address,

            }).ToList();

            // Return the list of renters as a successful response
            return Ok(renterList);
        }

        // GET: api/Renter/check-email
        // Checks if the provided email already exists in the database
        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmailUniqueness([FromQuery] string email)
        {
            // Validate the email if null or with white space
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required");
            }

            // Check if the email exists in the Renters table
            var existingRenter = await _context.Renters
                .FirstOrDefaultAsync(r => r.renter_email.ToLower() == email.ToLower());

             // Return true if the email is not already taken, otherwise false
            return Ok(existingRenter == null);
        }

        // POST: api/Renter/addRenter
        // Adds a new renter to the database
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
                )
            {
                return BadRequest("All fields are required.");
            }

            // Add the renter to the Renters table
            _context.Renters.Add(renter);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Now link the renter with the user by updating the user_id
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == renter.renter_email);
                if (user != null)
                {
                    renter.user_id = user.user_id; // Link renter with the user record
                    await _context.SaveChangesAsync();  // Save changes again after linking
                }

                // Return the newly created renter record as a response
                return CreatedAtAction(nameof(GetRenterList), new { id = renter.renter_id }, renter);
            }
            catch (Exception ex)
            {
                // Return a server error response if saving fails
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving renter data: " + ex.Message);
            }
        }

        // POST: api/Renter/addAcc
        // Adds a new account (user) to the system
        [HttpPost("addAcc")]
        public async Task<IActionResult> AddAccount([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("user data is null.");
            }

            // Ensure that the required fields for user creation are provided
            if (string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password) )
            {
                return BadRequest("All fields are required.");
            }

            // Add the renter data to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Return the newly created user account as a response
            return CreatedAtAction(nameof(GetRenterList), new { id = user.user_id }, user);

        }

    }
}