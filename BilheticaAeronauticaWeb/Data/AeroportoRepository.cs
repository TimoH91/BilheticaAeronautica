using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public class AeroportoRepository : GenericRepository<Aeroporto>, IAeroportoRepository
    {
        public AeroportoRepository(DataContext context) : base(context)
        {
        }
    }
}
