
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
        LblUserEmail.Text = Preferences.Get("username", string.Empty);
        LblUserName.Text = Preferences.Get("firstname", string.Empty);
        ImgBtnProfile.Source = await GetProfileImage();
        
    }

    private async Task<string?> GetProfileImage()
    {
        // Obtenha a imagem padr o do AppConfig
        string imagemPadrao = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        // Lida com casos de erro
        if (errorMessage != null)
        {
            switch (errorMessage)
            {
                //case "Unauthorized":
                //    if (!_loginPageDisplayed)
                //    {
                //        await DisplayLoginPage();
                //        return null;
                //    }
                //    break;
                //default:
                //    await DisplayAlert("Erro", errorMessage ?? "N o foi poss vel obter a imagem.", "OK");
                //    return imagemPadrao;
            }
        }

        if (response?.ImageUrl != null)
        {
            return response.ImageFullPath;
        }

        return imagemPadrao;
    }


    private async void MyAccount_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MyAccountPage(_apiService));
    }

    private async void MyTickets_Tapped(object sender, TappedEventArgs e)
    {
        //string token = Preferences.Get("accessToken", "");

        //var tickets = await _apiService.GetTicketsAsync(token);

        //TicketList.ItemsSource = tickets;

        await Navigation.PushAsync(new TicketsPage(_apiService));
    }



    private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();

            if (imageArray is null)
            {
                await DisplayAlert("Error", "It wasn't possible to load the image", "Ok");
                return;
            }

            ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadUserImage(imageArray);
            if (response.Data)
            {
                await DisplayAlert("", "Image uploaded successfully", "Ok");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "Unknown error occurred", "Cancel");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unknown error occurred: {ex.Message}", "Ok");
        }
    }

    private async Task<byte[]?> SelectImageAsync()
    {
        try
        {
            var arquivo = await MediaPicker.PickPhotoAsync();

            if (arquivo is null) return null;

            using (var stream = await arquivo.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "Device doesn't offer this function", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "Permission denied for media gallery", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error when selecting image: {ex.Message}", "Ok");
        }
        return null;
    }

    private async void Logout_Tapped(object sender, TappedEventArgs e)
    {
        Preferences.Remove("accesstoken");
        Preferences.Remove("userid");
        Preferences.Remove("username");
        Preferences.Remove("firstname");
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _basketService));
    }

    private async void About_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AuthorPage());
    }
}