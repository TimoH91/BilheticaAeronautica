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
    private bool _returnTicket;

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
        _returnTicket = false;
        BindingContext = this; 
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetDepartureSeats();
        //var departureSeats = await _apiService.GetSeatsByFlightAsync(_flight.Id);
        //DepartureSeats.Clear();
        //foreach (var seat in departureSeats)
        //    DepartureSeats.Add(seat);



        if (_returnFlight != null)
        {
            await GetReturnSeats();
            //var returnSeats = await _apiService.GetSeatsByFlightAsync(_returnFlight.Id);
            //ReturnSeats.Clear();
            //foreach (var seat in returnSeats)
            //    ReturnSeats.Add(seat);
        }
    }

    public async Task GetDepartureSeats()
    {
        var departureSeats = await _apiService.GetSeatsByFlightAsync(_flight.Id);
        DepartureSeats.Clear();
        foreach (var seat in departureSeats)
            DepartureSeats.Add(seat);
    }

    public async Task GetReturnSeats()
    {
        var returnSeats = await _apiService.GetSeatsByFlightAsync(_returnFlight.Id);
        ReturnSeats.Clear();
        foreach (var seat in returnSeats)
            ReturnSeats.Add(seat);
    }



    public async Task AddShoppingBasketTicket()
    {
        await ConfirmSeatSelection();

        var userId = Preferences.Get("userid", string.Empty);

        //TODO add seat to validation
        if (_returnTicket == false && SelectedDepartureSeat != null || _returnTicket == false && PickPassengerType.SelectedItem?.ToString() == "Infant")
        {
            if (await _validator.ValidateTicket(EntName.Text, EntSurname.Text, PickPassengerType.SelectedItem?.ToString(), PickClass.SelectedItem?.ToString()))
            {

                bool isInfant = await IsInfant(PickPassengerType.SelectedItem?.ToString());

                if (isInfant)
                {
                    SelectedDepartureSeat = new Seat();
                    SelectedDepartureSeat.Id = _basketService.InfantSeatId;
                }

                var flightTicket = new ShoppingBasketTicket
                {
                    Name = EntName.Text,
                    Surname = EntSurname.Text,
                    PassengerType = Enum.Parse<PassengerType>(PickPassengerType.SelectedItem.ToString()),
                    Class = Enum.Parse<TicketClass>(PickClass.SelectedItem.ToString()),
                    FlightId = _flight.Id,
                    Flight = _flight,
                    Price = _flight.BasePrice,
                    SeatId = SelectedDepartureSeat.Id,
                    IsResponsibleAdult = false,
                    UserId = userId
                };

                await _apiService.HoldSeat(SelectedDepartureSeat.Id);
                await GetDepartureSeats();
                _basketService.Add(flightTicket);
                await DisplayAlert("", $"Ticket for {flightTicket.Name} {flightTicket.Surname} added to shopping basket", "Ok");


                if (_returnFlight != null)
                {
                    _returnTicket = true;
                    await DisplayAlert("", $"Choose return seat.", "Ok");
                    EnableReturnPicker();
                }
                else
                {
                    await DisplayAlert("", $"Add another departure ticket if desired.", "Ok");
                }

                    SelectedDepartureSeat = null;
            }
            else
            {
                string errorMessage = "";
                errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
                errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
                errorMessage += _validator.TicketClassError != null ? $"\n- {_validator.TicketClassError}" : "";
                errorMessage += _validator.PassengerTypeError != null ? $"\n- {_validator.PassengerTypeError}" : "";

                await DisplayAlert("Error", errorMessage, "OK");
            }
        }

        //TODO need to get this to work for infants too!
        if (_returnTicket && SelectedReturnSeat != null || _returnTicket && PickPassengerType.SelectedItem?.ToString() == "Infant")
        {
            if (await _validator.ValidateTicket(EntName.Text, EntSurname.Text, PickPassengerType.SelectedItem?.ToString(), PickClass.SelectedItem?.ToString()))
            {
                bool isInfantReturn = await IsInfantReturn(PickPassengerType.SelectedItem?.ToString());

                if (isInfantReturn)
                {
                    SelectedReturnSeat = new Seat();
                    SelectedReturnSeat.Id = _basketService.InfantSeatId;
                }

                var returnFlightTicket = new ShoppingBasketTicket
                {
                    Name = EntName.Text,
                    Surname = EntSurname.Text,
                    PassengerType = Enum.Parse<PassengerType>(PickPassengerType.SelectedItem.ToString()),
                    Class = Enum.Parse<TicketClass>(PickClass.SelectedItem.ToString()),
                    FlightId = _returnFlight.Id,
                    Flight = _returnFlight,
                    Price = _returnFlight.BasePrice,
                    SeatId = SelectedReturnSeat.Id,
                    IsResponsibleAdult = false,
                    UserId = userId
                };

                _returnTicket = false;
                await _apiService.HoldSeat(SelectedReturnSeat.Id);
                await GetReturnSeats();
                SelectedReturnSeat = null;
                _basketService.Add(returnFlightTicket);
                await DisplayAlert("", $"Ticket for {returnFlightTicket.Name} {returnFlightTicket.Surname} added to shopping basket.", "Ok");
                await DisplayAlert("", $"Add another departure ticket if desired.", "Ok");
                DisableReturnPicker();
            }
            else
            {
                string errorMessage = "";
                errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
                errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
                errorMessage += _validator.TicketClassError != null ? $"\n- {_validator.TicketClassError}" : "";
                errorMessage += _validator.PassengerTypeError != null ? $"\n- {_validator.PassengerTypeError}" : "";

                await DisplayAlert("Error", errorMessage, "OK");
            }
        }
    }

    private async Task<bool> IsInfantReturn(string passengerType)
    {
        if (passengerType == "Infant")
        {
            var tcs = new TaskCompletionSource<bool>();


            await Navigation.PushAsync(new ResponsibleAdultPage(_basketService, _returnFlight.Id, () =>
            {
                tcs.SetResult(true);
            }));

            await tcs.Task;

            return true;
        }

        return false;
    }

    private async Task<bool> IsInfant(string passengerType)
    {
        if (passengerType == "Infant")
        {
            var tcs = new TaskCompletionSource<bool>();


            await Navigation.PushAsync(new ResponsibleAdultPage(_basketService, _flight.Id, () =>
            {
                tcs.SetResult(true); 
            }));

            await tcs.Task;

            return true;
        }

        return false;
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

    private async Task ConfirmSeatSelection()
    {

        if (PickPassengerType.SelectedItem?.ToString() == "Infant")
        {
            await DisplayAlert("Seat Selection", $"Infants use the same seat as the responsible adult", "OK");
        }

        if (SelectedReturnSeat != null)
        {
            await DisplayAlert("Seat Selected", $"You chose {SelectedReturnSeat.DisplayName}", "OK");
        }


        if (SelectedDepartureSeat != null && SelectedReturnSeat == null)
        {
            await DisplayAlert("Seat Selected", $"You chose {SelectedDepartureSeat.DisplayName}", "OK");
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
