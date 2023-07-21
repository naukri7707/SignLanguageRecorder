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

        // 處理 Unpack app icon font 遺失問題
        Icon.FixUnpackAppMissingFont("Resources/Fonts/MaterialDesignIcon.ttf", "Material Design Icons");
#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Data
        builder.Services.AddTransient<SecureStorageService>();
        builder.Services.AddSingleton<PreferencesService>();
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddTransient<RecorderLayoutService>();
        builder.Services.AddTransient<VocabularyService>();
        // Python
        builder.Services.AddSingleton<PythonService>();
        builder.Services.AddSingleton<JointsRecognizerService>();

        var app = builder.Build();

        return app;
    }
}
