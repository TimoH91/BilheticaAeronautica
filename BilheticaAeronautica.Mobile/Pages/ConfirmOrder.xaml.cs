using BilheticaAeronautica.Mobile.Models;
using BilheticaAeronautica.Mobile.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Pages;

public partial class ConfirmOrder : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IBasketService _basketService;

    public ConfirmOrder(ApiService apiService, IBasketService basketService)
    {
        InitializeComponent();
        _apiService = apiService;
        _basketService = basketService;
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

        await _apiService.ConfirmOrder(items);

        _basketService.Clear();
    }

    private void BtnEmptyBasket_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Empty basket clicked");
        _basketService.Clear(); 
    }
}
