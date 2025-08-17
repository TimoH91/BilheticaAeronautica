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
    public ObservableCollection<Seat> Seats { get; set; }
    public Seat SelectedSeat { get; set; }

    public FlightPage(ApiService apiService, IBasketService basketService, IValidator validator, Flight flight)
    {
        InitializeComponent();
        _apiService = apiService;
        _basketService = basketService;
        _validator = validator;
        _flight = flight;

        Seats = new ObservableCollection<Seat>();
        BindingContext = this; 
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        var seats = await _apiService.GetSeatsByFlightAsync(_flight.Id);

        Seats.Clear();
        foreach (var seat in seats)
            Seats.Add(seat);
    }

    public async void AddShoppingBasketTicket()
    {

        if (await _validator.ValidateTicket(EntName.Text, EntSurname.Text, PickPassengerType.SelectedItem?.ToString(), PickClass.SelectedItem?.ToString()))
        {
            var userId = Preferences.Get("userId", string.Empty);

            var entity = new ShoppingBasketTicket
            {
                Name = EntName.Text,
                Surname = EntSurname.Text,
                PassengerType = Enum.Parse<PassengerType>(PickPassengerType.SelectedItem.ToString()),
                Class = Enum.Parse<TicketClass>(PickClass.SelectedItem.ToString()),
                FlightId = _flight.Id,
                Price = _flight.BasePrice,
                SeatId = SelectedSeat.Id,
                IsResponsibleAdult = false,
                UserId = userId
            };

            _basketService.Add(entity);
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

    private void BtnSignIn_Clicked_2(object sender, EventArgs e)
    {
        AddShoppingBasketTicket();
    }

    private async void BtnFlights_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPage1(_apiService, _basketService, _validator));
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
        if (SelectedSeat != null)
        {
            DisplayAlert("Seat Selected", $"You chose {SelectedSeat.DisplayName}", "OK");
        }
    }
}
