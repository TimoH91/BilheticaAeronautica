using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;
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
            await DisplayAlert("Erro", "Enter your email", "Cancelar");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Erro", "Enter your password", "Cancelar");
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
        {
            await Navigation.PushAsync(new NewPage1(_apiService, _basketService, _validator));
            //Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            await DisplayAlert("Erro", "Algo deu errado", "Cancelar");
        }

    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {

        //await Navigation.PushAsync(new InscriptionPage(_apiService, _validator));

    }
}