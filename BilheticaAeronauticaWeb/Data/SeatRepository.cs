using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        private readonly DataContext _context;

        public SeatRepository(DataContext context) : base(context)
        {
             _context = context;
        }

    }
}
