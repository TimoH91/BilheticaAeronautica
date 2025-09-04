using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using System.Diagnostics;
using System.Text.Json;
using IValidator = BilheticaAeronautica.Mobile.Validations.IValidator;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class ConfirmOrder : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IBasketService _basketService;
    private readonly IValidator _validator;

    public ConfirmOrder(ApiService apiService, IBasketService basketService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _basketService = basketService;
        _validator = validator;
        BindingContext = _basketService;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        //ShoppingBasketTickets.ItemsSource = _basketService.Items;

        var username = Preferences.Get("username", "");

        if (String.IsNullOrEmpty(username))
        {
            RegisterUser.IsVisible = true;
        }
    }

    private async void BtnConfirmOrder_Clicked(object sender, EventArgs e)
    {
        var items = _basketService.Items.ToList();

        if (items.Count == 0)
        {
            await DisplayAlert("Basket Empty", "There are no tickets in your basket.", "OK");
            return;
        }

        var userId = Preferences.Get("userid", string.Empty);

        //TODO add register new user part here
        if (string.IsNullOrEmpty(userId))
        {
            if (await _validator.ValidateRegister(EntName.Text, EntSurname.Text, EntEmail.Text, EntPassword.Text))
            {
                var registerResponse = await _apiService.RegisterUser(EntName.Text, EntSurname.Text, EntEmail.Text, EntPassword.Text, EntConfirmPassword.Text);

                if (!registerResponse.Data)
                {
                    await DisplayAlert("Error", "User failed to register. Please try again later.", "OK");
                    return;
                }

                if (!registerResponse.HasError && registerResponse.RegisterUser != null)
                {

                    await DisplayAlert("", "User registered please check email for account confirmation.", "OK");

                    foreach (var item in items)
                    {
                        item.UserId = registerResponse.RegisterUser.UserId;
                    }

                    var orderResponse = await _apiService.ConfirmOrder(items);

                    if (!orderResponse.Data)
                    {
                        await DisplayAlert("Error", "Tickets failed to be purchased. Please try again later", "OK");
                        return;
                    }

                    await DisplayAlert("", "Ticket order confirmed!", "OK");

                    _basketService.Clear();
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
        else
        {
            foreach (var shoppingBasketTicket in items)
            {
                shoppingBasketTicket.UserId = userId;
                shoppingBasketTicket.Flight = null;
            }

            var orderResponse = await _apiService.ConfirmOrder(items);

            if (!orderResponse.Data)
            {
                await DisplayAlert("Error", "Tickets failed to be purchased. Please try again later", "OK");
                return;
            }

            await DisplayAlert("", "Ticket order confirmed!", "OK");

            _basketService.Clear();
        }

    }

    private void BtnEmptyBasket_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Empty basket clicked");
        _basketService.Clear();
    }
}
