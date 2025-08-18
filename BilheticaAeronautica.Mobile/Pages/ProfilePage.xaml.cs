
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly IValidator _validator;
    private readonly ApiService _apiService;
    private readonly IBasketService _basketService;
	public ProfilePage(IValidator validator, ApiService apiService, IBasketService basketService)
	{
		InitializeComponent();
        _validator = validator;
        _apiService = apiService;
        _basketService = basketService;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //ImgBtnPerfil.Source = await GetImagemPerfil();
        LblUserName.Text = Preferences.Get("username", string.Empty);
    }

    private void MyAccount_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async void MyTickets_Tapped(object sender, TappedEventArgs e)
    {
        //string token = Preferences.Get("accessToken", "");

        //var tickets = await _apiService.GetTicketsAsync(token);

        //TicketList.ItemsSource = tickets;

        await Navigation.PushAsync(new TicketsPage(_apiService));
    }

    private async void FAQ_Tapped(object sender, TappedEventArgs e)
    {


    }

    private async void BtnLogout_Clicked(object sender, EventArgs e)
    {

    }

    private void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {

    }

    private async void Logout_Tapped(object sender, TappedEventArgs e)
    {
        //var check = Preferences.Get("username", string.Empty);
        //var check3 = Preferences.Get("accesstoken", string.Empty);
        //var check4 = Preferences.Get("userid", string.Empty);
        Preferences.Remove("accesstoken");
        Preferences.Remove("userid");
        Preferences.Remove("username");
        //var check2 = Preferences.Get("username", string.Empty);
        //var check5 = Preferences.Get("accesstoken", string.Empty);
        //var check6 = Preferences.Get("userid", string.Empty);
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _basketService));
    }
}