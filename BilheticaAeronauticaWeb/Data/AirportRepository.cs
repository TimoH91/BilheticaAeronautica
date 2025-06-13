using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class AirportRepository : GenericRepository<Airport>, IAirportRepository
    {
        private readonly DataContext _context;

        public AirportRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Airport> GetAll()
        {
            return _context.Airports
                .Include(a => a.City)
                .Include(a => a.Country);
        }

        public IEnumerable<SelectListItem> GetComboAirports()
        {
            var list = _context.Airports.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select an airport...)",
                Value = "0"
            });

            return list;
        }
    }
}
