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
    }

    private async void BtnConfirmOrder_Clicked(object sender, EventArgs e)
    {
        var items = _basketService.Items.ToList();

        if (items.Count == 0)
        {
            await DisplayAlert("Basket Empty", "There are no tickets in your basket.", "OK");
            return;
        }

        //TODO add register new user part here
        if (string.IsNullOrEmpty(items[0].UserId))
        {
            var response = await _apiService.RegisterUser(EntName.Text, EntSurname.Text, EntEmail.Text, EntPassword.Text, EntConfirmPassword.Text);

            if (!response.HasError && response.RegisterUser != null)
            {
                foreach (var item in items)
                {
                    item.UserId = response.RegisterUser.UserId;
                }

                await _apiService.ConfirmOrder(items);

                _basketService.Clear();
            }
            else
            {
                string errorMessage = "";
                errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
                errorMessage += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
                errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
                errorMessage += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";

                await DisplayAlert("Erro", errorMessage, "OK");
            }
        }
    }

    private void BtnEmptyBasket_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Empty basket clicked");
        _basketService.Clear();
    }
}
