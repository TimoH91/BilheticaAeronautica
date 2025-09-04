using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;
using System.Threading.Tasks;
namespace BilheticaAeronautica.Mobile.Pages;


public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IBasketService _basketService;
    private readonly IValidator _validator;

    public LoginPage(ApiService apiService, IValidator validator, IBasketService basketService)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _basketService = basketService;
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Error", "Enter your email", "Cancelar");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Error", "Enter your password", "Cancelar");
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
        {
            await DisplayAlert("", "Logged in successfully!", "Ok");
            await Navigation.PushAsync(new FlightsPage(_apiService, _basketService, _validator));
            //Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong. Try again!", "Cancelar");
            await Task.Delay(200);
        }

    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {

        await Navigation.PushAsync(new RegisterPage(_apiService, _validator, _basketService));

    }

    private async void TapRecover_Tapped(object sender, TappedEventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Error", "Enter your email", "Cancelar");
            return;
        }

        var response =  await _apiService.RecoverPassword(EntEmail.Text);

        if (response.Data)
        {
            await DisplayAlert("", "Password recovery instructions have been sent to your email.", "Ok");
        }
    }
}