using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace autofleetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentedVehicleController : ControllerBase
    {
        private readonly AutoFleetDbContext _context;

        public RentedVehicleController(AutoFleetDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/RentedVehicle
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentedVehicle>>> GetRentedVehicles()
        {
            try
            {
                var rentedVehicles = await _context.RentedVehicles
                    .Select(rv => new 
                    {
                        rv.rented_vehicle_id,
                        rv.renter_id,
                        rv.vehicle_id,
                        rv.renter_fname,
                        rv.renter_lname,
                        rv.pickup_date,
                        rv.pickup_time,
                        rv.dropoff_date,
                        rv.dropoff_time,
                        rv.car_manufacturer,
                        rv.car_model,
                        rv.plate_number,
                        rv.created_at,
                        rv.updated_at,
                        rv.rent_status
                    })
                    .ToListAsync();

                return Ok(rentedVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving rented vehicles: " + ex.Message);
            }
        }

    // POST: api/RentedVehicle/add
    [HttpPost("add")]
    public async Task<IActionResult> AddRentedVehicle([FromBody] RentedVehicle rentedVehicle)
    {
    if (rentedVehicle == null)
    {
        return BadRequest("Invalid rented vehicle data.");
    }

    // Ensure renter_id is provided and exists
    var renter = await _context.Renters.FindAsync(rentedVehicle.renter_id);
    if (renter == null)
    {
        return BadRequest("Renter not found.");
    }

    rentedVehicle.renter_fname = renter.renter_fname;
    rentedVehicle.renter_lname = renter.renter_lname;

    // Ensure vehicle_id is provided and exists
    var vehicle = await _context.Vehicles.FindAsync(rentedVehicle.vehicle_id);
    if (vehicle == null)
    {
        return BadRequest("Vehicle not found.");
    }

    rentedVehicle.car_manufacturer = vehicle.car_manufacturer;
    rentedVehicle.car_model = vehicle.car_model;
    rentedVehicle.plate_number = vehicle.plate_number;

    // Set the creation and update timestamps
    rentedVehicle.created_at = DateTime.UtcNow;
    rentedVehicle.updated_at = DateTime.UtcNow;

    // Do not manually set rented_vehicle_id, it will be auto-generated
    rentedVehicle.rented_vehicle_id = 0;  

    
    rentedVehicle.rent_status = "Active";

   
    _context.RentedVehicles.Add(rentedVehicle);
    await _context.SaveChangesAsync();

    return Ok(rentedVehicle);
}


        // GET: api/RentedVehicle/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RentedVehicle>> GetRentedVehicle(int id)
        {
            var rentedVehicle = await _context.RentedVehicles
                .Where(rv => rv.rented_vehicle_id == id)
                .Select(rv => new
                {
                    rv.rented_vehicle_id,
                    rv.renter_id,
                    rv.vehicle_id,
                    rv.renter_fname,
                    rv.renter_lname,
                    rv.pickup_date,
                    rv.pickup_time,
                    rv.dropoff_date,
                    rv.dropoff_time,
                    rv.car_manufacturer,
                    rv.car_model,
                    rv.plate_number,
                    rv.created_at,
                    rv.updated_at,
                    rv.rent_status
                })
                .FirstOrDefaultAsync();

            if (rentedVehicle == null)
            {
                return NotFound();
            }

            return Ok(rentedVehicle);
        }

        // PUT: api/RentedVehicle/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRentedVehicle(int id, [FromBody] RentedVehicle rentedVehicle)
        {
            if (id != rentedVehicle.rented_vehicle_id)
            {
                return BadRequest("Vehicle ID mismatch.");
            }

            rentedVehicle.updated_at = DateTime.UtcNow;

            _context.Entry(rentedVehicle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentedVehicleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/RentedVehicle/latest-id
        [HttpGet("latest-id")]
        public async Task<IActionResult> GetLatestRentedVehicleId()
        {
            var latestId = await _context.RentedVehicles
                .OrderByDescending(rv => rv.rented_vehicle_id)  
                .Select(rv => rv.rented_vehicle_id)             
                .FirstOrDefaultAsync();

            return Ok(latestId);
        }

        // DELETE: api/RentedVehicle/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRentedVehicle(int id)
        {
            var rentedVehicle = await _context.RentedVehicles.FindAsync(id);

            if (rentedVehicle == null)
            {
                return NotFound();
            }

            _context.RentedVehicles.Remove(rentedVehicle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RentedVehicleExists(int id)
        {
            return _context.RentedVehicles.Any(e => e.rented_vehicle_id == id);
        }
    }
}
