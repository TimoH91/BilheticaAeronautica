using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class NewPage1 : ContentPage
{

    private readonly ApiService _apiService;

	public NewPage1(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
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
}