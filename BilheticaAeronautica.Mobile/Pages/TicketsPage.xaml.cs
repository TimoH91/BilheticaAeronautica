using BilheticaAeronautica.Mobile.Services;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class TicketsPage : ContentPage
{
	private readonly ApiService _apiService;
	public TicketsPage(ApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string token = Preferences.Get("accesstoken", "");

        var tickets = await _apiService.GetTicketsAsync(token);

        TicketList.ItemsSource = tickets;

    }
}