using BilheticaAeronautica.Mobile.Services;
namespace BilheticaAeronautica.Mobile.Pages;


public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    //private readonly IValidator _validator;

    public LoginPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Erro", "Informe o email", "Cancelar");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Erro", "Informe a senha", "Cancelar");
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
        {
            Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            await DisplayAlert("Erro", "Algo deu errado", "Cancelar");
        }

    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {

        await Navigation.PushAsync(new InscriptionPage(_apiService, _validator));

    }
}