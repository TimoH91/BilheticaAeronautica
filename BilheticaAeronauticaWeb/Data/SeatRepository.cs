using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BilheticaAeronauticaWeb.Data
{
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        private readonly DataContext _context;

        public SeatRepository(DataContext context) : base(context)
        {
             _context = context;
        }

        public async Task<Seat> GetByIdTrackedAsync(int id)
        { 
                return await _context.Seats
                 .Include(a => a.Flight)
                 .FirstOrDefaultAsync(a => a.Id == id);
        }

        public IEnumerable<SelectListItem> GetComboSeats()
        {

            var seats = _context.Seats.ToList(); 

            var list = seats
                .OrderBy(s => s.Row)
                .ThenBy(s => s.Column)
                .Select(s => new SelectListItem
                {
                    Text = $"{s.Row}-{s.Column}",
                    Value = s.Id.ToString()
                })
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a seat...)",
                Value = "0"
            });

            return list;
        }
        

        public async Task<IEnumerable<Seat>> GetAvailableSeatsByFlight(int flightId)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(f => f.Flight.Id == flightId && f.Occupied == false && f.IsHeld == false)
                .OrderBy(f => f.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GetAllSeatsByFlight(int flightId)
        {
            return await _context.Seats
                .Where(f => f.Flight.Id == flightId)
                .OrderBy(f => f.Id)
                .ToListAsync();
        }


    }
}
