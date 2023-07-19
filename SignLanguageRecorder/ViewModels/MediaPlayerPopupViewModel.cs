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

    [ObservableProperty]
    private string mediaSource;

    public MediaPlayerPopupViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<PreferencesService>()
        )
    { }

    public MediaPlayerPopupViewModel(IRequirement requirement, PreferencesService preferencesService)
    {
        this.requirement = requirement;
        this.preferencesService = preferencesService;
    }

    public void LoadDemo(string videoName)
    {
        var dataFolder = preferencesService.DataFolder;
        var fullPath = Path.Combine(dataFolder, "Demo", $"{videoName}.mp4");

        if (File.Exists(fullPath))
        {
            MediaSource = fullPath;
        }
        else
        {
            Application.Current.MainPage.DisplayAlert("錯誤", $"找不到影片\r\n{videoName}", "OK");
        }
    }
}
