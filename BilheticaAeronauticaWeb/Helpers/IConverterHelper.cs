using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Helpers
{
    public interface IConverterHelper
    {
        //TODO: Check which of these need tasks and which don't
            Country ToCountry(CountryViewModel model, Guid ImageId, bool isNew);

            CountryViewModel ToCountryViewModel(Country country);

            Task<Airport> ToAirport(AirportViewModel model, bool isNew);

            AirportViewModel ToAirportViewModel(Airport airport);

            Airplane ToAirplane(AirplaneViewModel model, Guid ImageId, bool isNew);

            AirplaneViewModel ToAirplaneViewModel(Airplane airplane);

            Task<Flight> ToFlight(FlightViewModel model, bool isNew);

            FlightViewModel ToFlightViewModel(Flight flight);

            TicketViewModel ToTicketViewModel(Ticket ticket);

            Ticket ToTicket(TicketViewModel model, bool isNew);

            ShoppingBasketTicket ToShoppingBasketTicket(TicketViewModel model, bool isNew);

            Task<Ticket> BasketToTicket(ShoppingBasketTicket basketTicket);

            Task<User> ToUser(UserViewModel model, bool isNew);
            Task<UserViewModel> ToUserViewModelAsync(User user);

            Order ToOrder(OrderViewModel model, bool isNew, User user);

            OrderViewModel ToOrderViewModel(Order order);

    }
}
