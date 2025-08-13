
using BilheticaAeronautica.Mobile.Validations;
using BilheticaAeronautica.Mobile.Pages;
using BilheticaAeronautica.Mobile.Services;

namespace BilheticaAeronautica.Mobile
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage1 = new NewPage1(_apiService);
            var homePage = new LoginPage(_apiService, _validator);

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent { Title = "Home",Icon = "home",Content = homePage  },
            }

            });
        }
    }
}
