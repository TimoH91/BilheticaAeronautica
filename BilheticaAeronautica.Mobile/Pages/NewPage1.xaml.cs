using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class NewPage1 : ContentPage
{

    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly IBasketService _basketService;
    private Flight _flight;
	public NewPage1(ApiService apiService, IBasketService basketService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _basketService = basketService;
        _validator = validator;
        _flight = new Flight();
	}

    private async void OnLoadTicketsClicked(object sender, EventArgs e)
    {
        string token = Preferences.Get("accessToken", "");

        var tickets = await _apiService.GetTicketsAsync(token);

        TicketList.ItemsSource = tickets;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await _apiService.RecoverPassword();
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (EntNewPassword.Text != EntConfirmPassword.Text)
        {
            await DisplayAlert("Error", "New password and confirm password do not match.", "OK");
            return;
        }

        await _apiService.ChangePassword(EntOldPassword.Text, EntNewPassword.Text, EntConfirmPassword.Text);
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        var flights = await _apiService.GetAllFlightsAsync();

        FlightList.ItemsSource = flights;   
    }

    //private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    //{
    //    _flight = (Flight)FlightList.SelectedItem;
    //}

    private void Button_Clicked_2(object sender, EventArgs e)
    {

    }

    private async void Button_Clicked_3(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FlightPage(_apiService, _basketService, _validator, _flight));
    }

    private async void BtnShoppingBasket_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FlightPage(_apiService, _basketService, _validator, _flight));
    }

    private void FlightList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Flight flight)
        {
            _flight = flight;
        }
    }
}