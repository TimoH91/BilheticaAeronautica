using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;

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


    private void OnOriginSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.Trim();

        if (!string.IsNullOrEmpty(searchText))
        {
            OriginAirportsList.ItemsSource = _allAirports
                .Where(a => a.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        else
        {
            OriginAirportsList.ItemsSource = new List<Airport>();
        }
    }

    private void OnDestinationSearchTextChanged(object sender, TextChangedEventArgs e)
    {

        var searchText = e.NewTextValue?.Trim();

        if (!string.IsNullOrEmpty(searchText))
        {
            DestinationAirportsList.ItemsSource = _allAirports
                .Where(a => a.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        else
        {
            DestinationAirportsList.ItemsSource = new List<Airport>();
        }
    }

    private void OnOriginAirportSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Airport airport)
        {
            DisplayAlert("Origin Selected", airport.Name, "OK");
        }
    }

    private void OnDestinationAirportSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Airport airport)
        {
            DisplayAlert("Destination Selected", airport.Name, "OK");
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

        FlightList.ItemsSource = flights;

        if (RoundTripSwitch.IsToggled && origin != null && destination != null)
        {
            var returnFlights = await _apiService.GetFlightsMobileAsync(destinationId, originId, returnDate);

            if (returnFlights.Any())
            {
                ReturnFlightsList.IsEnabled = true;
                ReturnFlightsList.ItemsSource = returnFlights;
            }
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
}