using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using Microsoft.AspNetCore.Identity;

namespace BilheticaAeronauticaWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userhelper)
        {
            _context = context;
            _userHelper = userhelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            var user = await _userHelper.GetUserByEmailAsync("timothyharris04@gmail.com"); 

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Timothy",
                    LastName = "Harris",
                    Email = "timothyharris04@gmail.com",
                    UserName = "timothyharris04@gmail.com",
                    PhoneNumber = "1234567890"

                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

            }

            if (!_context.Airports.Any())
            {
                AddAeroporto("Heathrow", "London", "United Kingdom");
                AddAeroporto("Luton", "London", "United Kingdom");
                AddAeroporto("Birmingham International", "Birmingham", "United Kingdom");
                AddAeroporto("Manchester", "Manchester", "United Kingdom");
                await _context.SaveChangesAsync();
            }
        }

        private void AddAeroporto(string name, string city, string country)
        {
            _context.Airports.Add(new Airport
            {
                Name = name,
                City= city,
                Country = country
            });

        }
    }
}
