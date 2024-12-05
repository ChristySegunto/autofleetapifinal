using Microsoft.AspNetCore.SignalR;

namespace autofleetapi.Hubs
{
    // Define the NotificationHub class that inherits from Hub, which enables SignalR communication
    public class NotificationHub : Hub
    {
        // Method to send a notification to all connected clients
        public async Task SendNotification(string message)
        {
            // Sends the "ReceiveNotification" message with the provided message to all connected clients
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}