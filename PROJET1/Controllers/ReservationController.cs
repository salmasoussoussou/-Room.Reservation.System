using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projet1.Data;
using projet1.mdl;
using projet1.models;
using projet1.DTO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace projet1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationContext _context;

        public ReservationController(ReservationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AllReservations()
        {
            var reservations = await _context.Reservations
                                             .Include(r => r.Room)
                                             .Select(r => new dtoreservation
                                             {
                                                 Id = r.Id,
                                                 RoomId = r.RoomId,
                                                 StartTime = r.StartTime,
                                                 EndTime = r.EndTime,
                                                 Room = new dtoroom
                                                 {
                                                     Id = r.Room.Id,
                                                  Name = r.Room.Name,
                                                    Capacity = r.Room.Capacity,
                                                   IsAvailable = r.Room.IsAvailable
                                                 }
                                            })
                                             .ToListAsync();
            return Ok(reservations);
        }


        

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _context.Reservations
                                            .Include(r => r.Room)
                                            .Where(r => r.Id == id)
                                            .Select(r => new dtoreservation
                                            {
                                                Id = r.Id,
                                                RoomId = r.RoomId,
                                                StartTime = r.StartTime,
                                                EndTime = r.EndTime,
                                                Room = new dtoroom
                                                {
                                                    Id = r.Room.Id,
                                                    Name = r.Room.Name,
                                                    Capacity = r.Room.Capacity,
                                                    IsAvailable = r.Room.IsAvailable
                                                }
                                            })
                                            .SingleOrDefaultAsync();

            if (reservation == null)
            {
                return NotFound($"Reservation with id {id} not found");
            }

            return Ok(reservation);
        }

        [HttpGet("reservationroom/{idroom}")]
        public async Task<IActionResult> GetReservationsByRoomId(int idroom)
        {
            var reservations = await _context.Reservations
                                             .Include(r => r.Room)
                                             .Where(r => r.RoomId == idroom)
                                             .Select(r => new dtoreservation
                                             {
                                                 Id = r.Id,
                                                 RoomId = r.RoomId,
                                                 StartTime = r.StartTime,
                                                 EndTime = r.EndTime,
                                                 Room = new dtoroom
                                                 {
                                                     Id = r.Room.Id,
                                                     Name = r.Room.Name,
                                                     Capacity = r.Room.Capacity,
                                                     IsAvailable = r.Room.IsAvailable
                                                 }
                                             })
                                             .ToListAsync();

            if (!reservations.Any())
            {
                return NotFound($"No reservations found for room with id {idroom}");
            }

            return Ok(reservations);
        }

        [HttpPost]
        // bach ndiro wa7d reservation fwa7d la salle  ilaa kant isavailable = true rah khawia n9dro nreserviw ama ila kant false faraha makhawyach man9droch nreserviw

        public async Task<IActionResult> PostReservation([FromBody] mdlreservation mdl)
        {
            var room = await _context.Rooms.FindAsync(mdl.RoomId);
            if (room == null)
            {
                return NotFound($"Room with id {mdl.RoomId} not found");
            }

            if (!room.IsAvailable)
            {
                return BadRequest($"Room with id {mdl.RoomId} is not available for reservation");
            }

            var reservation = new Reservation
            {
                StartTime = mdl.StartTime,
                EndTime = mdl.EndTime,
                RoomId = mdl.RoomId
            };

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, [FromBody] mdlreservation mdl)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound($"Reservation with id {id} not found");
            }

            var room = await _context.Rooms.FindAsync(mdl.RoomId);
            if (room == null)
            {
                return NotFound($"Room with id {mdl.RoomId} not found");
            }

            if (!room.IsAvailable)
            {
                return BadRequest("The room is not available for reservation");
            }

            reservation.RoomId = mdl.RoomId;
            reservation.StartTime = mdl.StartTime;
            reservation.EndTime = mdl.EndTime;

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound($"Reservation with id {id} not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound($"Reservation with id {id} not found");
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllReservations()
        {
            var reservations = await _context.Reservations.ToListAsync();
            if (reservations.Count == 0)
            {
                return NotFound(new { message = "No reservations found to delete" });
            }

            _context.Reservations.RemoveRange(reservations);
            await _context.SaveChangesAsync();

            return Ok(new { message = "All reservations deleted successfully" });
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
