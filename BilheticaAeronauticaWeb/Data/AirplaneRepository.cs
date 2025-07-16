using System.Runtime.InteropServices;
using AspNetCoreGeneratedDocument;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class AirplaneRepository : GenericRepository<Airplane>, IAirplaneRepository
    {
        private readonly DataContext _context;

        public AirplaneRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboAirplanes()
        {
            var list = _context.Airplanes.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select an airplane...)",
                Value = "0"
            });

            return list;
        }

        public async Task<IEnumerable<Airplane>> GetAvailableAirplanes(Flight flight)
        {
            var unavailableAirplaneIds = await _context.Flights.
                Where(f => f.Date == flight.Date && f.Id != flight.Id)
                .Select(f => f.AirplaneId)
                .Distinct()
                .ToListAsync();

            var availableAirplanes = await _context.Airplanes.
                Where(a => a.Status != false && !unavailableAirplaneIds.Contains(a.Id))
                .ToListAsync();

            return availableAirplanes;

        }
    }
}
