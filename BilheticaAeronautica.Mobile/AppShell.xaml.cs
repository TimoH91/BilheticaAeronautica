
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

        public AppShell(ApiService apiService, IValidator validator, IBasketService basketService)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));

            ConfigureShell();

        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator, _basketService);
            var flightsPage = new FlightsPage(_apiService, _basketService, _validator);
            var loginPage = new LoginPage(_apiService, _validator, _basketService);
            var myAccountPage = new MyAccountPage(_apiService);
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
                new ShellContent { Title = "Flights",Icon = "plane_icon",Content = flightsPage },
                new ShellContent { Title = "Profile",Icon = "profile",Content = profilePage },
            }

            });
        }
    }
}
