using System.Data;
using System.Threading.Tasks;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace BilheticaAeronauticaWeb.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IAirplaneRepository _airplaneRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        


        public ConverterHelper(ICountryRepository countryRepository, IAirplaneRepository airplaneRepository, IAirportRepository airportRepository, UserManager<User> userManager, IUserRepository userRepository, IFlightRepository flightRepository)
        {
            _countryRepository = countryRepository;
            _airplaneRepository = airplaneRepository;
            _airportRepository = airportRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _flightRepository = flightRepository;
        }

        public Country ToCountry(CountryViewModel model, Guid ImageId, bool isNew)
        {
            return new Country
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                FlagImageId = ImageId
            };
        }

        public CountryViewModel ToCountryViewModel(Country country)
        {
            return new CountryViewModel
            {
                Id = country.Id,
                Name = country.Name,
                FlagImageId = country.FlagImageId, 
            };
        }

        public async Task<Airport> ToAirport(AirportViewModel model, bool isNew)
        {
            return new Airport
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                City = await _countryRepository.GetCityAsync(model.CityId),
                Country = await _countryRepository.GetCountryAsync(model.CountryId)
            };
        }

        public AirportViewModel ToAirportViewModel(Airport airport)
        {
            return new AirportViewModel
            {
                Id = airport.Id,
                Name = airport.Name,
                CityId = airport.City.Id,
                CountryId = airport.Country.Id
            };
        }

        public Airplane ToAirplane(AirplaneViewModel model, Guid ImageId, bool isNew)
        {
            return new Airplane
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Manufacturer = model.Manufacturer,
                Rows = model.Rows,
                SeatsPerRow = model.SeatsPerRow,
                ImageId = ImageId,
                Status = model.Status
            };
        }

        public AirplaneViewModel ToAirplaneViewModel(Airplane airplane)
        {
            return new AirplaneViewModel
            {
                Id = airplane.Id,
                Name = airplane.Name,
                Manufacturer = airplane.Manufacturer,
                Rows = airplane.Rows,
                SeatsPerRow = airplane.SeatsPerRow,
                ImageId = airplane.ImageId,
                Status = airplane.Status
            };
        }

        public Flight ToFlight(FlightViewModel model, bool isNew)
        {
                return new Flight
                {
                    Id = isNew ? 0 : model.Id,
                    Date = model.Date,
                    Time = model.Time,
                    BasePrice = model.BasePrice,
                    Duration = model.Duration,
                    AirplaneId = model.AirplaneId,
                    OriginAirportId = model.OriginAirportId,    
                    DestinationAirportId = model.DestinationAirportId,
                    LayoverId = model.LayoverId,
                };
        }

        public FlightViewModel ToFlightViewModel(Flight flight)
        {
            return new FlightViewModel
            {
                Id = flight.Id,
                Date = flight.Date,
                Time = flight.Time,
                BasePrice = flight.BasePrice,
                Duration = flight.Duration,
                AirplaneId = flight.AirplaneId,
                OriginAirportId = flight.OriginAirportId,
                DestinationAirportId = flight.DestinationAirportId,
                LayoverId = flight.LayoverId,
            };
        }

        public TicketViewModel ToAdultAndChildTicketViewModel(Ticket ticket) 
        {
                return new TicketViewModel
                {
                    Id = ticket.Id,
                    Name = ticket.Name,
                    Surname = ticket.Surname,
                    FlightId = ticket.FlightId,
                    Class = ticket.Class,
                    OriginAirportId = ticket.OriginAirportId,
                    DestinationAirportId = ticket.DestinationAirportId,
                    SeatId = ticket.SeatId,
                    //Payment = ticket.Payment,
                    Price = ticket.Price,
                    OrderId = ticket.OrderId,
                    Type = ticket.Type,
                };
        }

        public TicketViewModel ToInfantTicketViewModel(InfantTicket ticket)
        {
            return new TicketViewModel
            {
                Id = ticket.Id,
                Name = ticket.Name,
                Surname = ticket.Surname,
                FlightId = ticket.FlightId,
                Class = ticket.Class,
                OriginAirportId = ticket.OriginAirportId,
                DestinationAirportId = ticket.DestinationAirportId,
                SeatId = ticket.SeatId,
                //Payment = ticket.Payment,
                Price = ticket.Price,
                Type = ticket.Type,
                OrderId = ticket.OrderId,
                ResponsibleAdultTicketId = ticket.ResponsibleAdultTicketId,
            };
        }


        public Ticket ToTicket(TicketViewModel model, bool isNew, Flight flight)
        {
                if (model.Type == PassengerType.Adult)
                {
                    return new AdultTicket
                    {
                        Id = isNew ? 0 : model.Id,
                        Name = model.Name,
                        Surname = model.Surname,
                        FlightId = flight.Id,
                        Class = model.Class,
                        OriginAirportId = flight.OriginAirportId,
                        DestinationAirportId = flight.DestinationAirportId,
                        SeatId = model.SeatId,
                        //Payment = model.Payment,
                        OrderId = model.OrderId.Value,
                        Price = model.Price,
                        Type = model.Type,
                        //IsResponsibleAdult = model.IsResponsibleAdult
                    };
                }

                if (model.Type == PassengerType.Child)
                {
                    return new ChildTicket
                    {
                        Id = isNew ? 0 : model.Id,
                        Name = model.Name,
                        Surname = model.Surname,
                        FlightId = flight.Id,
                        Class = model.Class,
                        OriginAirportId = flight.OriginAirportId,
                        DestinationAirportId = flight.DestinationAirportId,
                        SeatId = model.SeatId,
                        //Payment = model.Payment,
                        OrderId = model.OrderId.Value,
                        Price = model.Price,
                        Type = model.Type
                    };
                }

                return new InfantTicket
                {
                    Id = isNew ? 0 : model.Id,
                    Name = model.Name,
                    Surname = model.Surname,
                    FlightId = flight.Id,
                    Class = model.Class,
                    OriginAirportId = flight.OriginAirportId,
                    DestinationAirportId = flight.DestinationAirportId,
                    SeatId = model.SeatId,
                    //Payment = model.Payment,
                    Price = model.Price,
                    Type = PassengerType.Infant,
                    OrderId = model.OrderId.Value,
                    ResponsibleAdultTicketId = model.ResponsibleAdultTicketId.Value
                };
            
        }

            public async Task<UserViewModel> ToUserViewModelAsync(User user)
            {
                var roles = await _userManager.GetRolesAsync(user);

                return new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.PasswordHash,
                    Role = roles.FirstOrDefault() ?? "No role"
                };
            }

        public async Task<User> ToUser(UserViewModel model, bool isNew)
        {
            if (isNew)
            {
                return new User
                {
                    //Id = model.Id,
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Role = model.Role
                };
            }


            var user = await _userRepository.GetByIdAsync(model.Id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Role = model.Role;

            return user;
        
        }

        public ShoppingBasketTicket ToShoppingBasketTicket(TicketViewModel model, bool isNew)
        {
            if (model.SeatId != null)
            {
                return new ShoppingBasketTicket
                {
                    Id = isNew ? 0 : model.Id,
                    Name = model.Name,
                    Surname = model.Surname,
                    //Flight = flight,
                    FlightId = model.FlightId.Value,
                    Class = model.Class,
                    SeatId = model.SeatId.Value,
                    //Seat = seat,
                    Price = model.Price,
                    PassengerType = model.Type,
                    ResponsibleAdultTicketId = model.ResponsibleAdultTicketId,
                };
            }

            return new ShoppingBasketTicket
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Surname = model.Surname,
                FlightId = model.FlightId.Value,
                Class = model.Class,
                Price = model.Price,
                PassengerType = model.Type,
                ResponsibleAdultTicketId = model.ResponsibleAdultTicketId,
            };

        }

        public Ticket BasketToTicket(ShoppingBasketTicket basketTicket, Flight flight)
        {
            //var flight = await _flightRepository.GetByIdAsync(basketTicket.FlightId);

            if (basketTicket.PassengerType == PassengerType.Adult)
            {
                return new AdultTicket
                {
                    Name = basketTicket.Name,
                    Surname = basketTicket.Surname,
                    FlightId = basketTicket.FlightId,
                    OriginAirportId = flight.OriginAirportId,
                    DestinationAirportId = flight.DestinationAirportId,
                    SeatId = basketTicket.SeatId,
                    Class = basketTicket.Class,
                    Payment = Payment.Paid,
                    Price = basketTicket.Price,
                    Type = PassengerType.Adult,
                    IsResponsibleAdult = basketTicket.IsResponsibleAdult
                };
            }

            if (basketTicket.PassengerType == PassengerType.Child)
            {
                return new ChildTicket
                {
                    Name = basketTicket.Name,
                    Surname = basketTicket.Surname,
                    FlightId = basketTicket.FlightId,
                    OriginAirportId = flight.OriginAirportId,
                    DestinationAirportId = flight.DestinationAirportId,
                    SeatId = basketTicket.SeatId,
                    Class = basketTicket.Class,
                    Payment = Payment.Paid,
                    Price = basketTicket.Price,
                    Type = PassengerType.Child,
                };
            }

                return new InfantTicket
                {
                    Name = basketTicket.Name,
                    Surname = basketTicket.Surname,
                    FlightId = basketTicket.FlightId,
                    OriginAirportId = flight.OriginAirportId,
                    DestinationAirportId = flight.DestinationAirportId,
                    SeatId = basketTicket.SeatId,
                    Class = basketTicket.Class,
                    Payment = Payment.Paid,
                    Price = basketTicket.Price,
                    Type = PassengerType.Infant,
                    ResponsibleAdultTicketId = basketTicket.ResponsibleAdultTicketId.Value,
                };

        }

        public Order ToOrder(OrderViewModel model, bool isNew, User user)
        {
            return new Order
            {
                Id = isNew ? 0 : model.Id,
                User = user,
                OrderDate = model.OrderDate,
                Tickets = model.Tickets,
                TotalPrice = model.TotalPrice,
                Payment = model.Payment
            };
        }

        public OrderViewModel ToOrderViewModel(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UserId = order.User.Id,
                Tickets = (List<Ticket>)order.Tickets,
                TotalPrice = order.TotalPrice,
                Payment = order.Payment,
            };
        }

        public ShoppingBasketTicketViewModel ToShoppingBasketTicketViewModel(ShoppingBasketTicket basketTicket)
        {
            return new ShoppingBasketTicketViewModel
            {
                Id = basketTicket.Id,
                FlightId = basketTicket.FlightId,
                Flight = basketTicket.Flight,
                Class = basketTicket.Class,
                Name = basketTicket.Name,
                PassengerType = basketTicket.PassengerType,
                Price = basketTicket.Price,
                SeatId = basketTicket.SeatId,
                Surname = basketTicket.Surname,
                ResponsibleAdultTicketId = basketTicket.ResponsibleAdultTicketId,
                UserId = basketTicket.UserId
            };
        }

        public ShoppingBasketTicket ToShoppingBasketTicketFromModel(ShoppingBasketTicketViewModel model, bool isNew)
        {
            return new ShoppingBasketTicket
            {
                Id = isNew ? 0 : model.Id,
                FlightId = model.FlightId,
                Class = model.Class,
                Name = model.Name,
                PassengerType = model.PassengerType,
                Price = model.Price,
                SeatId = model.SeatId.Value,
                Surname = model.Surname,
                ResponsibleAdultTicketId = model.ResponsibleAdultTicketId,
                UserId = model.UserId
            };
        }
    }
}
