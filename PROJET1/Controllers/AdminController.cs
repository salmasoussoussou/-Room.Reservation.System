using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using projet1.models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using projet1.Data;
namespace projet1.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ReservationContext _context;

        public AdminController(UserManager<AppUser> userManager, ReservationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Actions pour les Rooms
        [HttpGet("rooms")]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return Ok(rooms);
        }

        [HttpGet("rooms/{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound(new { Message = "Room not found" });
            }
            return Ok(room);
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room room)
        {
            if (id != room.Id)
            {
                return BadRequest(new { Message = "Room ID mismatch" });
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound(new { Message = "Room not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound(new { Message = "Room not found" });
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Room deleted successfully" });
        }

        // Actions pour les Reservations
        [HttpGet("reservations")]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _context.Reservations.Include(r => r.Room).ToListAsync();
            return Ok(reservations);
        }

        [HttpGet("reservations/{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _context.Reservations.Include(r => r.Room).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound(new { Message = "Reservation not found" });
            }
            return Ok(reservation);
        }

        [HttpPost("reservations")]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("reservations/{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest(new { Message = "Reservation ID mismatch" });
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound(new { Message = "Reservation not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("reservations/{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { Message = "Reservation not found" });
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Reservation deleted successfully" });
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
