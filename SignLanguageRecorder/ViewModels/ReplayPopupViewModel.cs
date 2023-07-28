using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace SignLanguageRecorder.ViewModels;

public partial class ReplayPopupViewModel : ObservableObject
{
    public interface IRequirement
    {
    }

    private readonly IRequirement requirement;

    private readonly DialogService dialogService;

    private readonly PreferencesService preferencesService;

    private readonly JointsRecognizerService jointsRecognizerService;

    public ObservableCollection<string> Cameras { get; set; }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private int selectedCameraIndex;

    public string SelectedCamera => SelectedCameraIndex >= 0 && SelectedCameraIndex < Cameras.Count
        ? Cameras[SelectedCameraIndex]
        : "None";

    public string VideoName { get; set; }

    [ObservableProperty]
    private string mediaSource;

    public string SourceVideoPath
    {
        get
        {
            var userFolder = preferencesService.UsersFolder;
            var userName = preferencesService.UserName;
            return Path.Combine(userFolder, userName, "Source", SelectedCamera, $"{VideoName}.mp4");
        }
    }

    public string SkeletonVideoPath
    {
        get
        {
            var userFolder = preferencesService.UsersFolder;
            var userName = preferencesService.UserName;
            return Path.Combine(userFolder, userName, "Skeleton", SelectedCamera, $"{VideoName}.mp4");
        }
    }

    public ReplayPopupViewModel(IRequirement requirement, string videoName) : this(
        requirement,
        videoName,
        Dependency.Inject<DialogService>(),
        Dependency.Inject<PreferencesService>(),
        Dependency.Inject<JointsRecognizerService>()
        )
    { }

    public ReplayPopupViewModel(IRequirement requirement, string videoName, DialogService dialogService, PreferencesService preferencesService, JointsRecognizerService jointsRecognizerService)
    {
        this.requirement = requirement;
        VideoName = videoName;
        this.dialogService = dialogService;
        this.preferencesService = preferencesService;
        this.jointsRecognizerService = jointsRecognizerService;
        //
        Cameras = new ObservableCollection<string>(GetRecordedSignCameras());
    }


    public IEnumerable<string> GetRecordedSignCameras()
    {
        var sourceFolder = Path.Combine(preferencesService.UserFolder, "Source");

        Directory.CreateDirectory(sourceFolder);
        var camFolderPaths = Directory.GetDirectories(sourceFolder);

        foreach (var camFolder in camFolderPaths)
        {
            var targetFile = Path.Combine(camFolder, $"{VideoName}.mp4");
            if (File.Exists(targetFile))
            {
                var cam = Path.GetFileName(camFolder);
                yield return cam;
            }
        }
    }

    [RelayCommand]
    public void LoadSourceVideo()
    {
        LoadVideo(SourceVideoPath);
    }

    [RelayCommand]
    public async void LoadSkeletonVideo()
    {
        if (!File.Exists(SkeletonVideoPath))
        {
            try
            {
                IsBusy = true;
                var directoryPath = Path.GetDirectoryName(SkeletonVideoPath);
                Directory.CreateDirectory(directoryPath);
                await jointsRecognizerService.CreateSkeletonVideo(SourceVideoPath, SkeletonVideoPath);
                LoadVideo(SkeletonVideoPath);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                if (ex is ServiceIsBusyException serviceBusyException)
                {
                    await dialogService.DisplayAlert("訊息", serviceBusyException.Message, "確認");
                }
            }
        }
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
