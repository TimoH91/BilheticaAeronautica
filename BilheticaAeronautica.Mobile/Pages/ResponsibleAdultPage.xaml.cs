using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class ResponsibleAdultPage : ContentPage
{
    private readonly IBasketService _basketService;
    private readonly Action _onCompleted;

    public ResponsibleAdultPage(IBasketService basketService, Action onCompleted)
	{
		InitializeComponent();
		_basketService = basketService;
        _onCompleted = onCompleted;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = _basketService;

    }

    private async void BtnAdult_Clicked(object sender, EventArgs e)
    {
        ShoppingBasketTicket responsibleAdult = (ShoppingBasketTicket)ShoppingBasketTickets.SelectedItem;
        _basketService.InfantSeatId = responsibleAdult.SeatId.Value;
        //_basketService.ResponsibleAdultId = responsibleAdult.Id;
        responsibleAdult.IsResponsibleAdult = true;
        //will this update? or I need to find the ticket and replace

        _onCompleted?.Invoke();
        await Navigation.PopAsync();
    }

    private void ShoppingBasketTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    //Remember it needs to distinguish between return and outgoing flight for seats



}