
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
            var homePage1 = new NewPage1(_apiService, _basketService, _validator);
            var homePage = new LoginPage(_apiService, _validator, _basketService);
            //var page3 = new FlightPage(_apiService, _basketService, _validator);
            var page4 = new ConfirmOrder(_apiService, _basketService);

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent { Title = "Home",Icon = "home",Content = homePage1  },
            }

            });
        }
    }
}
