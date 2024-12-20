using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using autofleetapi.Hubs;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly AutoFleetDbContext _context; // Assuming your DB context is ApplicationDbContext

        public NotificationController(IHubContext<NotificationHub> hubContext, AutoFleetDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        // The NotificationRequest class is used to receive the data sent in the body of the POST request.
        public class NotificationRequest
        {
            public string Message { get; set; }
        }

        // The SendNotification method is responsible for checking the maintenance due date and sending notifications
        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            // Get the current date (ignores time)
            var currentDate = DateTime.Now.Date;  
            var startOfDay = currentDate;  // 12:00 AM of today
            var endOfDay = currentDate.AddDays(1);
            
            // Query your database to get all maintenance schedules that are due today
            var maintenanceSchedules = await _context.Maintenances
                .Where(m => m.maintenance_due_date >= startOfDay && m.maintenance_due_date < endOfDay)
                .ToListAsync();

            // If there are any schedules for today, send notifications
            if (maintenanceSchedules.Any())
            {
                foreach (var schedule in maintenanceSchedules)
                {
                    var message = $"Maintenance due for {schedule.car_model} today!";
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
                }
            }

            // Return Ok response
            return Ok();
        }
    }
}
