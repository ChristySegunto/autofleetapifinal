using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class LocationController : ControllerBase 
    {
        private readonly AutoFleetDbContext _context;

        public LocationController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Start a new trip: Create a new CarUpdate record
        [HttpPost("start-trip")]
        public async Task<IActionResult> StartTrip([FromBody] CarUpdate carUpdate)
        {
            if (carUpdate == null)
            {
                return BadRequest(new { Message = "Invalid CarUpdate data." });
            }

            // Validate that vehicle_id and renter_id exist in their respective tables
            var vehicle = await _context.Vehicles.SingleOrDefaultAsync(v => v.vehicle_id == carUpdate.vehicle_id);
            if (vehicle == null)
            {
                return BadRequest(new { Message = "Vehicle not found." });
            }

            var renter = await _context.Renters.SingleOrDefaultAsync(r => r.renter_id == carUpdate.renter_id);
            if (renter == null)
            {
                return BadRequest(new { Message = "Renter not found." });
            }

            // Validate that the rented_vehicle_id exists in the RentedVehicle table
            var rentedVehicle = await _context.RentedVehicles.SingleOrDefaultAsync(rv => rv.rented_vehicle_id == carUpdate.rented_vehicle_id);
            if (rentedVehicle == null)
            {
                return BadRequest(new { Message = "Rented vehicle not found." });
            }

            // Update the rent_status of the rented vehicle
            rentedVehicle.rent_status = "Ongoing";
            vehicle.vehicle_status = "Rented";
            // Set the current timestamp and status
            carUpdate.last_update = DateTime.UtcNow;
            carUpdate.carupdate_status = "Ongoing";

            // Add the new CarUpdate record to the database
            _context.CarUpdates.Add(carUpdate);
            await _context.SaveChangesAsync(); // Save to generate the auto-incremented carupdate_id

            // Return the response with the generated carupdate_id
            return Ok(new { Message = "Trip started successfully", carUpdate });
        }


        [HttpPut("end-trip/{rented_vehicle_id}")]
        public async Task<IActionResult> EndTrip([FromRoute] int rented_vehicle_id)
        {
            // Retrieve all updates with the same rented_vehicle_id
            var existingUpdates = await _context.CarUpdates
                .Where(u => u.rented_vehicle_id == rented_vehicle_id && u.carupdate_status == "Ongoing")
                .ToListAsync();

            // If no records are found
            if (!existingUpdates.Any())
            {
                return NotFound(new { Message = "No car updates found for the given rented vehicle ID." });
            }

            // Calculate the total distance travelled and total fuel consumption
            decimal totalDistanceTravelled = 0m;
            decimal totalFuelConsumption = 0m;
            decimal fuelEfficiency = 0.8m;  // Example value, assuming 12 liters per 100 km

            for (int i = 1; i < existingUpdates.Count; i++)
            {
                var previousUpdate = existingUpdates[i - 1];
                var currentUpdate = existingUpdates[i];

                if (previousUpdate.location_latitude.HasValue && previousUpdate.location_longitude.HasValue &&
                    currentUpdate.location_latitude.HasValue && currentUpdate.location_longitude.HasValue)
                {
                    // Convert latitudes and longitudes from decimal to radians
                    var lat1 = ToRadians((double)previousUpdate.location_latitude.Value);
                    var lon1 = ToRadians((double)previousUpdate.location_longitude.Value);
                    var lat2 = ToRadians((double)currentUpdate.location_latitude.Value);
                    var lon2 = ToRadians((double)currentUpdate.location_longitude.Value);

                    // Calculate the Haversine distance
                    var dLat = lat2 - lat1;
                    var dLon = lon2 - lon1;

                    var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                            Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);

                    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    var earthRadiusKm = 6371; // Radius of Earth in kilometers

                    var distance = earthRadiusKm * c; // Distance in kilometers
                    totalDistanceTravelled += (decimal)distance;
                }
            }

            // Calculate total fuel consumption based on distance and fuel efficiency
            totalFuelConsumption = (totalDistanceTravelled * fuelEfficiency) / 100; // Assuming liters per 100 km

            // Update the carupdate_status for all matching records to "Completed"
            foreach (var update in existingUpdates)
            {
                update.carupdate_status = "Completed"; // Mark the trip as completed
                update.last_update = DateTime.UtcNow;  // Update the last update timestamp
            }

            // Update the rent_status of the RentedVehicles table to "Completed"
            var rentedVehicle = await _context.RentedVehicles
                                                .SingleOrDefaultAsync(rv => rv.rented_vehicle_id == rented_vehicle_id);
            if (rentedVehicle != null)
            {
                rentedVehicle.rent_status = "Completed";

                // Update the vehicle status to "Available"
                var vehicle = await _context.Vehicles
                                            .SingleOrDefaultAsync(v => v.vehicle_id == rentedVehicle.vehicle_id);
                if (vehicle != null)
                {
                    vehicle.vehicle_status = "Available";  // Set the vehicle status to "Available"

                    // Update the Vehicle's totals
                    vehicle.total_mileage = (vehicle.total_mileage ?? 0m) + totalDistanceTravelled;
                    vehicle.total_fuel_consumption = (vehicle.total_fuel_consumption ?? 0m) + totalFuelConsumption;
                    vehicle.updated_at = DateTime.UtcNow;
                }
                else
                {
                    return NotFound(new { Message = "Vehicle not found." });
                }
            }
            else
            {
                return NotFound(new { Message = "Rented vehicle not found." });
            }

            // Save all the changes to the database
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Trip completed successfully",
                RentStatus = rentedVehicle.rent_status,
                VehicleStatus = _context.Vehicles
                            .Where(v => v.vehicle_id == rentedVehicle.vehicle_id)
                            .Select(v => v.vehicle_status)
                            .FirstOrDefault(),
                TotalDistanceTravelled = totalDistanceTravelled,
                TotalFuelConsumption = totalFuelConsumption
            });
        }


        // GET: api/Location/realtime/{rentedVehicleId}
        [HttpGet("realtime/{rentedVehicleId}")]
        public async Task<IActionResult> GetRealTimeCarLocation(int rentedVehicleId)
        {
            // Fetch the car update for the given rented vehicle ID
            var carUpdate = await _context.CarUpdates
                .Where(cu => cu.rented_vehicle_id == rentedVehicleId)
                .OrderByDescending(cu => cu.last_update) 
                .FirstOrDefaultAsync();

            if (carUpdate == null)
            {
                return NotFound(new { message = "The update not found." });
            }


            // Create and return the DTO
            var carLocation = new RealTimeCarLocationDTO
            {
                LocationLatitude = carUpdate.location_latitude ?? 0m,  // Default to 0.0 if NULL
                LocationLongitude = carUpdate.location_longitude ?? 0m, // Default to 0.0 if NULL
                Speed = carUpdate.speed ?? 0m,  // Default to 0.0 if NULL
                TotalFuelConsumption = carUpdate.total_fuel_consumption ?? 0m,  // Default to 0.0 if NULL
                TotalDistanceTravelled = carUpdate.total_distance_travelled ?? 0m,  // Default to 0.0 if NULL
                CarUpdateStatus = carUpdate.carupdate_status
            };

            return Ok(carLocation);
        }

        [HttpGet("trip-summary/{rentedVehicleId}")]
        public async Task<IActionResult> GetTripSummary(int rentedVehicleId)
        {
            var carUpdates = await _context.CarUpdates
                .Where(cu => cu.rented_vehicle_id == rentedVehicleId)
                .OrderByDescending(cu => cu.last_update) 
                .ToListAsync();
            
            if (carUpdates == null || carUpdates.Count < 2)
            {
                return NotFound(new { message = "Insufficient data to compute distance and fuel consumption." });
            }

            decimal totalDistanceTravelled = 0m;
            decimal fuelEfficiency = 0.8m; //0.8 Liter per 100km

            for (int i = 1; i < carUpdates.Count; i++)
            {
                var previousUpdate = carUpdates[i - 1];
                var currentUpdate = carUpdates[i];

                if (previousUpdate.location_latitude.HasValue && previousUpdate.location_longitude.HasValue &&
                    currentUpdate.location_latitude.HasValue && currentUpdate.location_longitude.HasValue)
                {
                    // Convert latitudes and longitudes from decimal to radians
                    var lat1 = ToRadians((double)previousUpdate.location_latitude.Value);
                    var lon1 = ToRadians((double)previousUpdate.location_longitude.Value);
                    var lat2 = ToRadians((double)currentUpdate.location_latitude.Value);
                    var lon2 = ToRadians((double)currentUpdate.location_longitude.Value);

                    // Calculate the Haversine distance
                    var dLat = lat2 - lat1;
                    var dLon = lon2 - lon1;

                    var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                            Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);

                    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    var earthRadiusKm = 6371; // Radius of Earth in kilometers

                    var distance = earthRadiusKm * c; // Distance in kilometers
                    totalDistanceTravelled += (decimal)distance;
                }
            }

            // Calculate total fuel consumption based on distance and fuel efficiency
            var totalFuelConsumption = (totalDistanceTravelled * fuelEfficiency) / 100; // Assuming liters per 100 km


            // Return the computed summary
            return Ok(new
            {
                RentedVehicleId = rentedVehicleId,
                TotalDistanceTravelled = totalDistanceTravelled,
                TotalFuelConsumption = totalFuelConsumption,
            });

        }

        // Helper method to convert degrees to radians
        private double ToRadians(double angleInDegrees)
        {
            return angleInDegrees * (Math.PI / 180);
        }
    }

    


    
}