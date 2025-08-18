
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly IBasketService _basketService;
	public HomePage(ApiService apiService, IValidator validator, IBasketService basketService)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _basketService = basketService;
	}

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _basketService));
    }

    private async void BtnRegister_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_apiService, _validator, _basketService));
    }

    private async void BtnFlights_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FlightsPage(_apiService, _basketService, _validator));
    }
}