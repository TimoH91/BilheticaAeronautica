using BilheticaAeronautica.Mobile.Pages;
using BilheticaAeronautica.Mobile.Services;

namespace BilheticaAeronautica.Mobile
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        public AppShell(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new NewPage1(_apiService);

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
