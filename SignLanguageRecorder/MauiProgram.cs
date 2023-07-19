using Camera.MAUI;
using Microsoft.Extensions.Logging;

namespace SignLanguageRecorder;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement()
            .UseMauiCameraView()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialDesignIcon.ttf", Icon.UsingFontFamilyName);
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<PreferencesService>();

        builder.Services.AddTransient<SecureStorageService>();
        builder.Services.AddSingleton<DatabaseService>();

        builder.Services.AddTransient<PythonService>();
        builder.Services.AddTransient<RecorderLayoutService>();
        builder.Services.AddTransient<VocabularyService>();
        var app = builder.Build();

        return app;
    }
}
