using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Airport> Airports { get; set; }

        public DbSet<Airplane>  Airplanes { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<Seat> Seats { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .HasIndex(c => c.Name)
                .IsUnique();


            modelBuilder.Entity<Flight>()
                .Property(f => f.BasePrice)
                .HasColumnType("decimal(18,2)");



            modelBuilder.Entity<Ticket>()
    .HasDiscriminator<string>("TicketType")
    .HasValue<AdultTicket>("Adult")
    .HasValue<ChildTicket>("Child")
    .HasValue<InfantTicket>("Infant");

            modelBuilder.Entity<Ticket>()
.Property(f => f.Price)
.HasColumnType("decimal(18,2)");

            foreach (var foreignKey in modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
        //public DbSet<BilheticaAeronauticaWeb.Models.UserViewModel> UserViewModel { get; set; } = default!;
    }
}
