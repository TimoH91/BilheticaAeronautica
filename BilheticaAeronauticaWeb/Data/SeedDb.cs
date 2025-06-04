using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context; 
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Aeroportos.Any())
            {
                AddProduct("Heathrow", "London", "Reino Unido");
                AddProduct("Luton", "London", "Reino Unido");
                AddProduct("Birmingham International", "Birmingham", "Reino Unido");
                AddProduct("Manchester", "Manchester", "Reino Unido");
                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string nome, string cidade, string pais)
        {
            _context.Aeroportos.Add(new Aeroporto
            {
                Nome = nome,
                Cidade= cidade,
                Pais = pais
            });

        }
    }
}
