using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;

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

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Staff");
            await _userHelper.CheckRoleAsync("Customer");

            if (!_context.Countries.Any())
            {

                var flagId = Guid.NewGuid();
                
                var sourcePath = Path.Combine("SeedImages", "england.jpeg"); 
                var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "flags", $"{flagId}.jpg");
       
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                File.Copy(sourcePath, destinationPath, overwrite: true);

                var cities = new List<City>();
                cities.Add(new City { Name = "Birmingham" });
                cities.Add(new City { Name = "London" });
                cities.Add(new City { Name = "Manchester" });

                _context.Countries.Add(new Country
                {
                    FlagImageId = flagId,
                    Cities = cities,
                    Name = "England"
                });

                await _context.SaveChangesAsync();
            }

            var user = await _userHelper.GetUserByEmailAsync("timothyharris04@gmail.com"); 
            var user2 = await _userHelper.GetUserByEmailAsync("johnSmith@yopmail.com");
            var user3 = await _userHelper.GetUserByEmailAsync("sarahparker@yopmail.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Timothy",
                    LastName = "Harris",
                    Email = "timothyharris04@gmail.com",
                    UserName = "timothyharris04@gmail.com",
                    PhoneNumber = "1234567890",
                    Role = "Admin"

                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            if (user2 == null)
            {
                user2 = new User
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "johnSmith@yopmail.com",
                    UserName = "johnSmith@yopmail.com",
                    PhoneNumber = "1234567890",
                    Role = "Customer"

                };

                var result = await _userHelper.AddUserAsync(user2, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user2, "Customer");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user2);
                await _userHelper.ConfirmEmailAsync(user2, token);

            }

            if (user3 == null)
            {
                user3 = new User
                {
                    FirstName = "Sarah",
                    LastName = "Parker",
                    Email = "sarahparker@yopmail.com",
                    UserName = "sarahparker@yopmail.com",
                    PhoneNumber = "1234567890",
                    Role = "Staff"

                };

                var result = await _userHelper.AddUserAsync(user3, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user3, "Staff");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user3);
                await _userHelper.ConfirmEmailAsync(user3, token);

            }

            var isInRole1 = await _userHelper.IsUserInRoleAsync(user, "Admin");
            var isInRole2 = await _userHelper.IsUserInRoleAsync(user2, "Customer");
            var isInRole3 = await _userHelper.IsUserInRoleAsync(user3, "Staff");

            if (!isInRole1)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }            
            
            if (!isInRole2)
            {
                await _userHelper.AddUserToRoleAsync(user2, "Customer");
            }

            if (!isInRole3)
            {
                await _userHelper.AddUserToRoleAsync(user3, "Staff");
            }

            var englandCountry = _context.Countries.Include(c => c.Cities).FirstOrDefault(c => c.Name == "England");

            if (englandCountry != null && !_context.Airports.Any())
            {
                var londonCity = englandCountry.Cities.FirstOrDefault(c => c.Name == "London");
                var birminghamCity = englandCountry.Cities.FirstOrDefault(c => c.Name == "Birmingham");
                var manchesterCity = englandCountry.Cities.FirstOrDefault(c => c.Name == "Manchester");

                AddAirport("Heathrow", londonCity, englandCountry);
                AddAirport("Luton", londonCity, englandCountry);
                AddAirport("Birmingham International", birminghamCity, englandCountry);
                AddAirport("Manchester", manchesterCity, englandCountry);

                await _context.SaveChangesAsync();
            }

            if (!_context.Airplanes.Any()) 
            {
                var imageId = Guid.NewGuid();


                var sourcePath = Path.Combine("SeedImages", "boeing_737.jpeg");
                var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "airplanes", $"{imageId}.jpg");


                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                File.Copy(sourcePath, destinationPath, overwrite: true);

                var airplane = new Airplane
                {
                    Name = "737",
                    Manufacturer = "Boeing",
                    SeatsPerRow = 7,
                    Rows = 30,
                    ImageId = imageId,
                    Status = true
                };

                _context.Airplanes.Add(airplane);
                await _context.SaveChangesAsync();
            }

            if (!_context.Flights.Any() && _context.Airplanes.Any() && _context.Airports.Any())
            {
                var originAirport = _context.Airports.FirstOrDefault(a => a.Id == 1);
                var destinationAirport = _context.Airports.FirstOrDefault(a => a.Id == 3);
                var airplane = _context.Airplanes.FirstOrDefault(a => a.Id == 1);

                var flight = new Flight
                {
                    Date = DateTime.Today.AddYears(1),
                    Time = new TimeSpan(13, 0, 0),
                    Duration = 60,
                    BasePrice = 50,
                    OriginAirport = originAirport,
                    DestinationAirport = destinationAirport,
                    Airplane = airplane
                };

                _context.Flights.Add(flight);
                await _context.SaveChangesAsync();
            }

            if (!_context.Seats.Any() && _context.Flights.Any())
            {
                var airplane = _context.Airplanes.FirstOrDefault(a => a.Id == 1);
                var flight = _context.Flights.FirstOrDefault(a => a.Id == 1);

                if (airplane == null)
                {
                    throw new Exception("Airplane not found.");
                }
                if (flight == null)
                {
                    throw new Exception("Flight not found.");
                }

                var seats = new List<Seat>();

                for (int row = 1; row <= airplane.Rows; row++)
                {
                    for (int col = 1; col <= airplane.SeatsPerRow; col++)
                    {
                        seats.Add(new Seat
                        {
                            Flight = flight,
                            Row = row,
                            Column = col,
                            Occupied = false
                        });
                    }
                }

                _context.Seats.AddRange(seats);
                await _context.SaveChangesAsync();
            }

            if (!_context.Orders.Any())
            {
                Order order = new Order
                {
                    OrderDate = DateTime.Now,
                    User = user2,
                    TotalPrice = 100,
                    Payment = Payment.Paid,
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }

            if (!_context.Tickets.Any() && _context.Seats.Any())
            {
                var originAirport = _context.Airports.FirstOrDefault(a => a.Id == 1);
                var destinationAirport = _context.Airports.FirstOrDefault(a => a.Id == 3);
                var flight = _context.Flights.FirstOrDefault(a => a.Id == 1);
                var seat1 = _context.Seats.FirstOrDefault(a => a.Id == 1);
                var seat2 = _context.Seats.FirstOrDefault(a => a.Id == 2);
                var order = _context.Orders.FirstOrDefault(a => a.Id == 1); 

                var adultTicket = new AdultTicket
                {
                    User = user2,
                    Name = "John",
                    Surname = "Smith",
                    OriginAirport = originAirport,
                    DestinationAirport = destinationAirport,
                    Flight = flight,
                    Seat = seat1,
                    Payment = Payment.Paid,
                    Class = TicketClass.Economic,
                    Price = 50,
                    Type = PassengerType.Adult,
                    Order = order
                    
                };

                var childTicket = new ChildTicket
                {
                    User = user2,
                    Name = "Lucy",
                    Surname = "Smith",
                    OriginAirport = originAirport,
                    DestinationAirport = destinationAirport,
                    Flight = flight,
                    Seat = seat2,
                    Payment = Payment.Paid,
                    Class = TicketClass.Economic,
                    Price = 50,
                    Order = order,
                    Type = PassengerType.Child
                };

                seat1.Occupied = true;
                seat2.Occupied = true;

                _context.Tickets.Add(adultTicket);
                _context.Tickets.Add(childTicket);
                await _context.SaveChangesAsync();
            }
        }

        private void AddAirport(string name, City city, Country country)
        {
            _context.Airports.Add(new Airport
            {
                Name = name,
                City = city,
                Country = country
            });

        }
    }
}
