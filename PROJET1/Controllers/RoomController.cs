using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projet1.Data;
using projet1.mdl;
using projet1.models;

namespace projet1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class RoomController : ControllerBase
    {
        private readonly ReservationContext _context;

        public RoomController(ReservationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var salle = await _context.Rooms.ToListAsync();
            return Ok(salle);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom(int id)
        {
            var room = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == id);
            if (room == null)
            {
                return NotFound($"Room with id {id} not exists");
            }

            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> PostRoom([FromBody] mdlroom mdl)
        {
            if (mdl.Capacity < 1 || mdl.Capacity > 500)
            {
                return BadRequest("Capacity must be between 1 and 500.");
            }
            var room = new Room
            {
                Name = mdl.Name,
                Capacity = mdl.Capacity,
                IsAvailable = mdl.IsAvailable,
            };
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return Ok(room);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, [FromBody] mdlroom mdl)
        {
            if (mdl.Capacity < 1 || mdl.Capacity > 500)
            {
                return BadRequest("Capacity must be between 1 and 500.");
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound($"Room with id {id} not exists");
            }

            room.Name = mdl.Name;
            room.Capacity = mdl.Capacity;
            room.IsAvailable = mdl.IsAvailable;

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound($"Room with id {id} not exists");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            if (rooms.Count == 0)
            {
                return NotFound(new { message = "No rooms found to delete" });
            }

            _context.Rooms.RemoveRange(rooms);
            await _context.SaveChangesAsync();

            return Ok(new { message = "All rooms deleted successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == id);
            if (room == null)
            {
                return NotFound($"Room with id {id} not exists");
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
