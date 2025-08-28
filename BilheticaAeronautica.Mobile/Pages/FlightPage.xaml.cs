using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class FlightPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly IBasketService _basketService;
    private readonly Flight _flight;
    private readonly Flight _returnFlight;
    private bool _departureAdded;

    //public Seat InfantSeat { get; set; }
    //public int ResponsibleAdultId { get; set; }
    public ObservableCollection<Seat> DepartureSeats { get; set; }
    public ObservableCollection<Seat> ReturnSeats { get; set; }
    public Seat SelectedDepartureSeat { get; set; }
    public Seat SelectedReturnSeat { get; set; }

    public FlightPage(ApiService apiService, IBasketService basketService, IValidator validator, Flight flight, Flight? returnFlight)
    {
        InitializeComponent();
        _apiService = apiService;
        _basketService = basketService;
        _validator = validator;
        _flight = flight;

        if (returnFlight != null)
        {
            _returnFlight = returnFlight;
        }

        DepartureSeats = new ObservableCollection<Seat>();
        ReturnSeats = new ObservableCollection<Seat>();
        _departureAdded = false;
        BindingContext = this; 
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        var departureSeats = await _apiService.GetSeatsByFlightAsync(_flight.Id);
        DepartureSeats.Clear();
        foreach (var seat in departureSeats)
            DepartureSeats.Add(seat);



        if (_returnFlight != null)
        {
            var returnSeats = await _apiService.GetSeatsByFlightAsync(_returnFlight.Id);
            ReturnSeats.Clear();
            foreach (var seat in returnSeats)
                ReturnSeats.Add(seat);
        }
    }

    public async Task AddShoppingBasketTicket()
    {
        ConfirmSeatSelection();


        //TODO add seat to validation
        if (await _validator.ValidateTicket(EntName.Text, EntSurname.Text, PickPassengerType.SelectedItem?.ToString(), PickClass.SelectedItem?.ToString()))
        {

           await  IsInfant(PickPassengerType.SelectedItem?.ToString());

            int infant = _basketService.InfantSeatId;

            var userId = Preferences.Get("userid", string.Empty);

            if (_departureAdded == false)
            {
                var flightTicket = new ShoppingBasketTicket
                {
                    Name = EntName.Text,
                    Surname = EntSurname.Text,
                    PassengerType = Enum.Parse<PassengerType>(PickPassengerType.SelectedItem.ToString()),
                    Class = Enum.Parse<TicketClass>(PickClass.SelectedItem.ToString()),
                    FlightId = _flight.Id,
                    Price = _flight.BasePrice,
                    SeatId = SelectedDepartureSeat.Id,
                    IsResponsibleAdult = false,
                    UserId = userId
                };

                await _apiService.HoldSeat(SelectedDepartureSeat.Id);
                _basketService.Add(flightTicket);

                    _departureAdded = true;
                    SelectedDepartureSeat = null;
                    EnableReturnPicker();
                
            }

            if (_departureAdded && SelectedReturnSeat != null)
            {
                IsInfant(PickPassengerType.SelectedItem?.ToString());

                var returnFlightTicket = new ShoppingBasketTicket
                {
                    Name = EntName.Text,
                    Surname = EntSurname.Text,
                    PassengerType = Enum.Parse<PassengerType>(PickPassengerType.SelectedItem.ToString()),
                    Class = Enum.Parse<TicketClass>(PickClass.SelectedItem.ToString()),
                    FlightId = _returnFlight.Id,
                    Price = _returnFlight.BasePrice,
                    SeatId = SelectedReturnSeat.Id,
                    IsResponsibleAdult = false,
                    UserId = userId
                };

                _departureAdded = false;
                SelectedReturnSeat = null;
                await _apiService.HoldSeat(SelectedReturnSeat.Id);
                _basketService.Add(returnFlightTicket);
                DisableReturnPicker();
            }
        }
        else
        {
            string errorMessage = "";
            errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMessage += _validator.TicketClassError != null ? $"\n- {_validator.TicketClassError}" : "";
            errorMessage += _validator.PassengerTypeError != null ? $"\n- {_validator.PassengerTypeError}" : "";

            await DisplayAlert("Erro", errorMessage, "OK");
        }   
    }

    private async Task IsInfant(string passengerType)
    {
        if (passengerType == "Infant")
        {
            var tcs = new TaskCompletionSource<bool>();


            await Navigation.PushAsync(new ResponsibleAdultPage(_basketService, () =>
            {
                tcs.SetResult(true); 
            }));

            await tcs.Task;
        }
    }
    private async void BtnFlights_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FlightsPage(_apiService, _basketService, _validator));
    }

    private async void Purchase_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ConfirmOrder(_apiService, _basketService, _validator));
    }

    private void BtnEmptyBasket_Clicked(object sender, EventArgs e)
    {
        _basketService.Clear(); // automatically saved
    }

    private void ConfirmSeatSelection()
    {
        if (SelectedReturnSeat != null)
        {
            DisplayAlert("Seat Selected", $"You chose {SelectedReturnSeat.DisplayName}", "OK");
        }


        if (SelectedDepartureSeat != null && SelectedReturnSeat == null)
        {
            DisplayAlert("Seat Selected", $"You chose {SelectedDepartureSeat.DisplayName}", "OK");
        }
    }

    private void EnableReturnPicker()
    { 

            ReturnSeatPicker.IsVisible = true;
            DepartureSeatPicker.IsVisible = false;

    }

    private void DisableReturnPicker()
    {
            ReturnSeatPicker.IsVisible = false;
            DepartureSeatPicker.IsVisible = true;

    }

    private async void BtnShoppingBasket_Clicked(object sender, EventArgs e)
    {
        await AddShoppingBasketTicket();
    }
}
