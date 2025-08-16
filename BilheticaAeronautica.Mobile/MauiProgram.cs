using Microsoft.Extensions.Logging;
using BilheticaAeronautica.Mobile.Services;
using BilheticaAeronautica.Mobile.Validations;

namespace BilheticaAeronautica.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();

#endif
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<IValidator, Validator>();
            builder.Services.AddSingleton<IBasketService, BasketService>();
            return builder.Build();

        }
    }
}
