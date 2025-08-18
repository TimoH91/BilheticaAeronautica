
using BilheticaAeronautica.Mobile.Validations;
using BilheticaAeronautica.Mobile.Pages;
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Models;

namespace BilheticaAeronautica.Mobile
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IBasketService _basketService;
        private readonly IValidator _validator;
        //private readonly Flight _flight;
        public AppShell(ApiService apiService, IValidator validator, IBasketService basketService)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
            //_flight = flight;
            ConfigureShell();

        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator, _basketService);
            var flightsPage = new FlightsPage(_apiService, _basketService, _validator);
            var loginPage = new LoginPage(_apiService, _validator, _basketService);
            //var page3 = new FlightPage(_apiService, _basketService, _validator);
            var registerPage = new RegisterPage(_apiService, _validator, _basketService);
            var confirmOrderPage = new ConfirmOrder(_apiService, _basketService, _validator);
            var profilePage = new ProfilePage(_validator, _apiService, _basketService);
            var ticketsPage = new TicketsPage(_apiService);

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent { Title = "Home",Icon = "home",Content = homePage },
                new ShellContent { Title = "Basket",Icon = "cart",Content = confirmOrderPage },
                new ShellContent { Title = "Flights",Icon = "profile",Content = flightsPage },
                new ShellContent { Title = "Profile",Icon = "profile",Content = profilePage },
            }

            });
        }
    }
}
