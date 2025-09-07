
using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class RegisterPage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IBasketService _basketService;
	private readonly IValidator _validator;

	public RegisterPage(ApiService apiService, IValidator validator, IBasketService basketService)
	{
		InitializeComponent();
		_apiService = apiService;
		_validator = validator;
	}

    private async void BtnRegister_Clicked(object sender, EventArgs e)
    {
		if (await _validator.ValidateRegister(EntName.Text, EntSurname.Text, EntEmail.Text, EntPassword.Text))
		{
			var response = await _apiService.RegisterUser(EntName.Text, EntSurname.Text, EntEmail.Text, EntPassword.Text, EntConfirmPassword.Text);

            if (!response.HasError)
            {
                await DisplayAlert("", "Please check your email to conclude registration.", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService,  _validator, _basketService));
            }
        }
        else
        {
            string errorMessage = "";
            errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMessage += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";

            await DisplayAlert("Error", errorMessage, "OK");
        }
    }
}