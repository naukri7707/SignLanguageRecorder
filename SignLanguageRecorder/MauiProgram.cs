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
        // 不需要加上路徑 (Resources/Fonts/MaterialDesignIcon.ttf) 因為這裡是抓建置後的位置，而字形檔建置後會被複製到與執行檔 (.exe) 同級的資料夾
        Icon.FixUnpackAppMissingFontFamily(@"MaterialDesignIcon.ttf", "Material Design Icons");
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
