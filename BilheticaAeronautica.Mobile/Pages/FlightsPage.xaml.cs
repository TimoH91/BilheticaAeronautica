using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;
using Microsoft.Maui.Layouts;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class FlightsPage : ContentPage
{

    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly IBasketService _basketService;
    private IEnumerable<Airport> _allAirports;
    private Flight _flight;
    private bool departureDatePicked = false;
    private bool returnDatePicked = false;
    public FlightsPage(ApiService apiService, IBasketService basketService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _basketService = basketService;
        _validator = validator;
        _flight = new Flight();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_allAirports == null)
        {
            _allAirports = await _apiService.GetAllAirportsAsync();
        }
    }

    private void OnOriginSearchFocused(object sender, FocusEventArgs e)
    {
        OriginAirportsList.ItemsSource = _allAirports;
        OriginAirportsList.IsVisible = true;
    }

    private void OnDestinationSearchFocused(object sender, FocusEventArgs e)
    {
        DestinationAirportsList.ItemsSource = _allAirports;
        DestinationAirportsList.IsVisible = true;
    }

    private void OnOriginSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.Trim();

        if (!string.IsNullOrEmpty(searchText))
        {
            var filtered = _allAirports
           .Where(a => a.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
           .ToList();

            OriginAirportsList.ItemsSource = filtered;
            OriginAirportsList.IsVisible = filtered.Any();
        }
        else
        {
            OriginAirportsList.ItemsSource = new List<Airport>();
            OriginAirportsList.IsVisible = true;
        }
    }

    private void OnDestinationSearchTextChanged(object sender, TextChangedEventArgs e)
    {

        var searchText = e.NewTextValue?.Trim();

        if (!string.IsNullOrEmpty(searchText))
        {
                var filtered = _allAirports
               .Where(a => a.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
               .ToList();

            DestinationAirportsList.ItemsSource = filtered;
            DestinationAirportsList.IsVisible = filtered.Any();
        }
        else
        {
            DestinationAirportsList.ItemsSource = new List<Airport>();
            DestinationAirportsList.IsVisible = true;
        }
    }

    private void OnOriginAirportSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Airport airport)
        {
            DisplayAlert("Origin Selected", airport.Name, "OK");

            OriginSearchBar.Text = airport.Name;

            OriginAirportsList.IsVisible = false;

            //OriginAirportsList.SelectedItem = null;
        }
    }

    private void OnDestinationAirportSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Airport airport)
        {
            DisplayAlert("Destination Selected", airport.Name, "OK");

            DestinationSearchBar.Text = airport.Name;

            DestinationAirportsList.IsVisible = false;

            //DestinationAirportsList.SelectedItem = null;
        }
    }

    private void FlightList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Flight flight)
        {
            _flight = flight;
        }

        ReturnFlightsList.IsEnabled = true;
        ReturnFlightsList.Opacity = 1;
    }

    private async void BtnSelectFlight_Clicked(object sender, EventArgs e)
    {
        //var departure = FlightList.SelectedItem as Flight;
        //var returnFlight = ReturnFlightsList.SelectedItem as Flight;

        var departure = (Flight)FlightList.SelectedItem;
        var returnFlight = (Flight)ReturnFlightsList.SelectedItem;

        if (departure != null && returnFlight == null)
        {
            await DisplayAlert("Flights Selected",
                $"Departure: {departure.Date}",
                "OK");

            await Navigation.PushAsync(new FlightPage(_apiService, _basketService, _validator, departure, null));
        }
        else if (departure != null && returnFlight != null)
        {
            await DisplayAlert("Flights Selected",
            $"Departure: {departure.Date}\nReturn: {returnFlight.Date}",
            "OK");

            await Navigation.PushAsync(new FlightPage(_apiService, _basketService, _validator, departure, returnFlight));
        }
    }

    private async void BtnLoadFlights_Clicked(object sender, EventArgs e)
    {
        Airport origin = (Airport)OriginAirportsList.SelectedItem;
        Airport destination = (Airport)DestinationAirportsList.SelectedItem;

        int? originId = origin?.Id;               
        int? destinationId = destination?.Id;

        DateTime? departureDate = departureDatePicked ? DepartureDatePicker.Date : (DateTime?)null;
        DateTime? returnDate = returnDatePicked ? ReturnDatePicker.Date : (DateTime?)null;

        var flights = await _apiService.GetFlightsMobileAsync(originId, destinationId, departureDate);

        if (!flights.Any())
        {
            await DisplayAlert("Error", "No flights available for your desired journey.", "Ok");
        }

        FlightList.ItemsSource = flights;

        if (RoundTripSwitch.IsToggled && origin != null && destination != null)
        {
            var returnFlights = await _apiService.GetFlightsMobileAsync(destinationId, originId, returnDate);

            if (returnFlights.Any())
            {
                ReturnFlightsList.IsEnabled = true;
                ReturnFlightsSelector.IsVisible = true;
                ReturnFlightsList.ItemsSource = returnFlights;
            }
            else
            {
                await DisplayAlert("Error", "No flights available for your desired return journey.", "Ok");
            }
        }

        if (flights.Any())
        {
            OriginSearchBar.IsVisible = false;
            DestinationSearchBar.IsVisible = false;
            DepartureDatePicker.IsVisible = false;
            ReturnDatePicker.IsVisible = false;
            BtnLoadFlights.IsVisible = false;
            RoundTripSwitch.IsVisible = false;
            RoundTripLbl.IsVisible = false;
            DepartureFlightsLbl.IsVisible = true;
            BtnSelectFlight.IsVisible = true;
        }

    }

    private void DepartureDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        departureDatePicked = true;
    }

    private void ReturnDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {

    }

    private void OnReturnFlightSelected(object sender, SelectionChangedEventArgs e)
    {
        // Enable confirm button only when both are selected
        if (ReturnFlightsList.SelectedItem != null && ReturnFlightsList.SelectedItem != null)
        {
            BtnSelectFlight.IsEnabled = true;
        }
    }

    private void RoundTripSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        ReturnDatePicker.IsVisible = e.Value;
    }

    private void DateTripSwitch_Toggled(object sender, ToggledEventArgs e)
    {

    }

    private void DestinationSearchBar_Focused(object sender, FocusEventArgs e)
    {

    }
}