using System.Net.Http.Headers;
using System.Text.Json;
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

    //private async void OnLoadTicketsClicked(object sender, EventArgs e)
    //{
    //    //string token = await SecureStorage.GetAsync("auth_token"); // Use your real token

    //    //TODO use the authentication later when user accounts are set up
    //    string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huU21pdGhAeW9wbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiam9oblNtaXRoQHlvcG1haWwuY29tIiwianRpIjoiODcxMmQ0YWYtYmRlNy00YTM2LTg3MDUtOTIzMGY3MzczYmI0IiwiZXhwIjoxNzU1MzMzOTgyLCJpc3MiOiJodHRwczovL2JpbGhldGljYWFlcm9uYXV0aWNhd2ViLWZnYnhnYWg4aDRoa2NtZnoud2VzdGV1cm9wZS0wMS5henVyZXdlYnNpdGVzLm5ldCIsImF1ZCI6Imh0dHBzOi8vYmlsaGV0aWNhYWVyb25hdXRpY2F3ZWItZmdieGdhaDhoNGhrY21mei53ZXN0ZXVyb3BlLTAxLmF6dXJld2Vic2l0ZXMubmV0In0.H-xkw79KWWrk5EVlpDPDi755UE2_73hdt772xh8BeVM";
    //    var client = new HttpClient
    //    {
    //        BaseAddress = new Uri("https://bilheticaaeronauticaweb-fgbxgah8h4hkcmfz.westeurope-01.azurewebsites.net/") // Change if needed
    //    };
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    //    var response = await client.GetAsync("api/tickets");

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var content = await response.Content.ReadAsStringAsync();
    //        var tickets = JsonSerializer.Deserialize<List<Ticket>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    //        TicketList.ItemsSource = tickets;
    //    }
    //    else
    //    {
    //        await DisplayAlert("Error", $"Failed: {response.StatusCode}", "OK");
    //    }
    //}

    private async void OnLoadTicketsClicked(object sender, EventArgs e)
    {
        string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huU21pdGhAeW9wbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiam9oblNtaXRoQHlvcG1haWwuY29tIiwianRpIjoiODcxMmQ0YWYtYmRlNy00YTM2LTg3MDUtOTIzMGY3MzczYmI0IiwiZXhwIjoxNzU1MzMzOTgyLCJpc3MiOiJodHRwczovL2JpbGhldGljYWFlcm9uYXV0aWNhd2ViLWZnYnhnYWg4aDRoa2NtZnoud2VzdGV1cm9wZS0wMS5henVyZXdlYnNpdGVzLm5ldCIsImF1ZCI6Imh0dHBzOi8vYmlsaGV0aWNhYWVyb25hdXRpY2F3ZWItZmdieGdhaDhoNGhrY21mei53ZXN0ZXVyb3BlLTAxLmF6dXJld2Vic2l0ZXMubmV0In0.H-xkw79KWWrk5EVlpDPDi755UE2_73hdt772xh8BeVM";

        var tickets = await _apiService.GetTicketsAsync(token);

        TicketList.ItemsSource = tickets;
    }
}