using BilheticaAeronautica.Mobile.Services;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;

	public MyAccountPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

	
    private async void BtnChangePassword_Clicked(object sender, EventArgs e)
    {
        if (EntNewPassword.Text != EntConfirmPassword.Text)
        {
            await DisplayAlert("Error", "New password and confirm password do not match.", "OK");
            return;
        }

        var response = await _apiService.ChangePassword(EntOldPassword.Text, EntNewPassword.Text, EntConfirmPassword.Text);

        if (response.Data)
        {
            await DisplayAlert("", "Password changed successfully!", "Ok");
        }
    }

    private async void BtnChangeInfo_Clicked(object sender, EventArgs e)
    {
        if (EntName.Text == null && EntSurname.Text == null)
        {
            await DisplayAlert("Error", "Please enter a new first or last name", "OK");
            return;
        }

        var response = await _apiService.ChangeUserInfo(EntName.Text, EntSurname.Text);

        if (response.Data)
        {
            await DisplayAlert("", "Info changed successfully!", "Ok");
        }
    }



    //private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        var imagemArray = await SelectImagemAsync();

    //        if (imagemArray is null)
    //        {
    //            await DisplayAlert("Erro", "N o foi poss vel carregar a imagem", "Ok");
    //            return;
    //        }

    //        ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imagemArray));

    //        var response = await _apiService.UploadUserImage(imagemArray);

    //        if (response.Data)
    //        {
    //            await DisplayAlert("", "Imagem enviada com sucesso", "Ok");
    //        }
    //        else
    //        {
    //            await DisplayAlert("Erro", response.ErrorMessage ?? "Ocorreu um erro desconhecido", "Cancela");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "Ok");
    //    }
    //}

    //private async Task<byte[]?> SelectImagemAsync()
    //{
    //    try
    //    {
    //        var arquivo = await MediaPicker.PickPhotoAsync();

    //        if (arquivo is null) return null;

    //        using (var stream = await arquivo.OpenReadAsync())
    //        using (var memoryStream = new MemoryStream())
    //        {
    //            await stream.CopyToAsync(memoryStream);
    //            return memoryStream.ToArray();
    //        }
    //    }
    //    catch (FeatureNotSupportedException)
    //    {
    //        await DisplayAlert("Erro", "A funcionalidade n o   suportada no dispositivo", "Ok");
    //    }
    //    catch (PermissionException)
    //    {
    //        await DisplayAlert("Erro", "Permiss es n o concedidas para acessar a c mera ou galeria", "Ok");
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Erro", $"Erro ao selecionar a imagem: {ex.Message}", "Ok");
    //    }
    //    return null;
    //}
}