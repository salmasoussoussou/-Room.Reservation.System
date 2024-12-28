using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using projet1.models;
using System.Collections.Generic;

namespace projet1.Data
{
    public class ReservationContext : IdentityDbContext<AppUser>
    {
        public ReservationContext(DbContextOptions<ReservationContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        //public DbSet<AppUser> appUsers { get; set; }

      //  protected override void OnModelCreating(ModelBuilder builder)
      //  {
        //    base.OnModelCreating(builder);
        //}
    }

}
