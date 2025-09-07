using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class ResponsibleAdultPage : ContentPage
{
    private readonly IBasketService _basketService;
    private readonly Action _onCompleted;
    private readonly int _flightId;

    public ResponsibleAdultPage(IBasketService basketService, int FlightId, Action onCompleted)
	{
		InitializeComponent();
		_basketService = basketService;
        _onCompleted = onCompleted;
        _flightId = FlightId;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var adultTickets = _basketService.Items.Where(i => i.PassengerType == PassengerType.Adult && i.FlightId == _flightId && i.IsResponsibleAdult == false).ToList();

        BindingContext = adultTickets;

        if (!adultTickets.Any())
        {
            await DisplayAlert("Error", "Please add adult tickets before infants.", "Ok");
            await Navigation.PopAsync();
        }
    }



    private async void BtnAdult_Clicked(object sender, EventArgs e)
    {
        if (ShoppingBasketTickets.SelectedItem == null)
        {
            await DisplayAlert("Error", "No adult ticket has been selected.", "OK");
            return;
        }


        if (ShoppingBasketTickets.SelectedItem != null)
        {
            ShoppingBasketTicket responsibleAdult = (ShoppingBasketTicket)ShoppingBasketTickets.SelectedItem;
            _basketService.InfantSeatId = responsibleAdult.SeatId.Value;
            responsibleAdult.IsResponsibleAdult = true;
            _onCompleted?.Invoke();
            await Navigation.PopAsync();
        }

        

    }

    private void ShoppingBasketTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    //Remember it needs to distinguish between return and outgoing flight for seats



}