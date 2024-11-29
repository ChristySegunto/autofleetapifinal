using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class LoginController : ControllerBase 
    {
        private readonly AutoFleetDbContext _context;

        public LoginController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // POST: api/login/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new { Message = "Invalid user data." });
            }

            var foundUser = _context.Users.SingleOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (foundUser != null)
            {
                if (foundUser.Role == "renter")
                {
                    // Find the renter associated with the user
                    var renter = _context.Renters.SingleOrDefault(r => r.user_id == foundUser.user_id);

                    if (renter != null)
                    {
                        // Count the number of rented vehicles for this renter
                        var rentedVehicleCount = _context.RentedVehicles.Count(rv => rv.renter_id == renter.renter_id);
                        var upcomingRentCount = _context.RentedVehicles.Count(rv => rv.renter_id == renter.renter_id && rv.pickup_date > DateTime.Now);

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
                        return NotFound(new { Message = "Renter not found for the logged-in user." });
                    }
                }
                else
                {
                    return Ok(new { Message = "Login successful!", Role = foundUser.Role, Email = foundUser.Email, UserId = foundUser.user_id });
                }
            }
            return Unauthorized(new { Message = "Invalid credentials." });
        }


        [HttpOptions("login")]
        public IActionResult Options()
        {
            return Ok(); // Respond to preflight request
        }

    }
}