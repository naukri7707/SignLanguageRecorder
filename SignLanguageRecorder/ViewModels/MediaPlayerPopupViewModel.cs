using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace SignLanguageRecorder.ViewModels;

public partial class MediaPlayerPopupViewModel : ObservableObject
{
    public interface IRequirement
    {
    }

    private readonly IRequirement requirement;

    private readonly PreferencesService preferencesService;

    private readonly DialogService dialogService;

    [ObservableProperty]
    private string mediaSource;

    public MediaPlayerPopupViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<PreferencesService>(),
        Dependency.Inject<DialogService>()
        )
    { }

    public MediaPlayerPopupViewModel(IRequirement requirement, PreferencesService preferencesService, DialogService dialogService)
    {
        this.requirement = requirement;
        this.preferencesService = preferencesService;
        this.dialogService = dialogService;
    }

    public void LoadDemo(string videoName)
    {
        var demoFolder = preferencesService.DemoFolder;
        var fullPath = Path.Combine(demoFolder, $"{videoName}.mp4");
        LoadVideo(fullPath);
    }

    public async void LoadVideo(string videoPath)
    {
        if (File.Exists(videoPath))
        {
            MediaSource = videoPath;
        }
        else
        {
            await dialogService.DisplayAlert("錯誤", $"找不到影片\r\n{videoPath}", "確認");
        }
    }
}
