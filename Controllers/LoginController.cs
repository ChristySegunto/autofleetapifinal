using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    // Define the base route for the LoginController
    [Route("api/[controller]")]
    [ApiController]
    
    public class LoginController : ControllerBase 
    {
        // Database context to interact with the Users and Renters tables
        private readonly AutoFleetDbContext _context;

        // Constructor that accepts AutoFleetDbContext to initialize the context
        public LoginController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Ensure context is not null
        }

        // POST: api/login/login
        // Handles login logic for both renters and admin
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            // Check if the incoming user data is valid (non-null and has non-empty email and password)
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new { Message = "Invalid user data." }); // Return bad request if user data is incomplete
            }

            // Try to find a user in the database with matching email and password
            var foundUser = _context.Users.SingleOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            // If a user is found, continue to check their role
            if (foundUser != null)
            {
                // If the user is a renter, fetch renter-specific information
                if (foundUser.Role == "renter")
                {
                    // Find the renter associated with the user
                    var renter = _context.Renters.SingleOrDefault(r => r.user_id == foundUser.user_id);

                    // If the renter exists, gather additional renter-related details
                    if (renter != null)
                    {
                        // Count the number of rented vehicles for this renter
                        var rentedVehicleCount = _context.RentedVehicles.Count(rv => rv.renter_id == renter.renter_id);

                        // Count how many upcoming rentals the renter has
                        var upcomingRentCount = _context.RentedVehicles.Count(rv => rv.renter_id == renter.renter_id && rv.rent_status == "Upcoming");

                        // Return the login success response with renter-specific details
                        return Ok(new
                        {
                            Message = "Login successful!",
                            Role = foundUser.Role,
                            Email = foundUser.Email,
                            UserId = foundUser.user_id,
                            RenterId = renter.renter_id,
                            RenterFname = renter.renter_fname,
                            RenterLname = renter.renter_lname,
                            RentedVehicleCount = rentedVehicleCount,
                            UpcomingRentCount = upcomingRentCount
                        });
                    }
                    else
                    {
                        // If no renter is found for the user, return a not found responses
                        return NotFound(new { Message = "Renter not found for the logged-in user." });
                    }
                }
                else
                {
                    // If the user is not a renter, just return basic user information
                    return Ok(new { Message = "Login successful!", Role = foundUser.Role, Email = foundUser.Email, UserId = foundUser.user_id });
                }
            }
            return Unauthorized(new { Message = "Invalid credentials." });
        }


        // HTTP OPTIONS request for login (used for handling CORS preflight requests)
        [HttpOptions("login")]
        public IActionResult Options()
        {
            return Ok(); // Respond to preflight request
        }

    }
}