using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    }
}
