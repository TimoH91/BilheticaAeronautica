using BilheticaAeronautica.Mobile.Pages;
using BilheticaAeronautica.Mobile.Services;

namespace BilheticaAeronautica.Mobile
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;

        public App(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            //NavigationPage navPage1 = new NavigationPage(new NewPage1());

            //MainPage = navPage1;

            SetMainPage();
        }

        private void SetMainPage()
        {
            //var accessToken = Preferences.Get("accesstoken", string.Empty);

            //if (string.IsNullOrEmpty(accessToken))
            //{
            //    MainPage = new NavigationPage(new InscriptionPage(_apiService, _validator));
            //    return;
            //}

            MainPage = new AppShell(_apiService);
        }
    }
}
