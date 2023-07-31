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

    public ObservableCollection<string> CameraNames { get; set; }

    [ObservableProperty]
    private bool isSkeletonShow;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string selectedCameraName;

    public string SignName { get; set; }

    [ObservableProperty]
    private string mediaSource;

    public ReplayPopupViewModel(IRequirement requirement, string signName) : this(
        requirement,
        signName,
        Dependency.Inject<DialogService>(),
        Dependency.Inject<PreferencesService>(),
        Dependency.Inject<JointsRecognizerService>()
        )
    { }

    public ReplayPopupViewModel(IRequirement requirement, string signName, DialogService dialogService, PreferencesService preferencesService, JointsRecognizerService jointsRecognizerService)
    {
        this.requirement = requirement;
        SignName = signName;
        this.dialogService = dialogService;
        this.preferencesService = preferencesService;
        this.jointsRecognizerService = jointsRecognizerService;
        //
        var userFolder = preferencesService.UsersFolder;
        var userName = preferencesService.UserName;
        var folderPath = Path.Combine(userFolder, userName, "Source");
        var files = Directory.GetFiles(folderPath, $"{signName}_*.mp4")
            .Select(it =>
            {
                var fileName = Path.GetFileNameWithoutExtension(it);
                var trueName = fileName.Replace($"{signName}_", null);
                return trueName;
            });
        CameraNames = new ObservableCollection<string>(files);
        SelectedCameraName = CameraNames.FirstOrDefault();
    }

    public IEnumerable<string> GetRecordedSignCameras()
    {
        var sourceFolder = Path.Combine(preferencesService.UserFolder, "Source");

        Directory.CreateDirectory(sourceFolder);
        var camFolderPaths = Directory.GetDirectories(sourceFolder);

        foreach (var camFolder in camFolderPaths)
        {
            var targetFile = Path.Combine(camFolder, $"{SignName}.mp4");
            if (File.Exists(targetFile))
            {
                var cam = Path.GetFileName(camFolder);
                yield return cam;
            }
        }
    }

    public async void UpdateVideo()
    {
        var targetVideoName = GetVideoPath(SignName, SelectedCameraName, IsSkeletonShow);

        if (!TryLoadVideo(targetVideoName))
        {
            if (IsSkeletonShow)
            {
                try
                {
                    var sourceVideo = GetVideoPath(SignName, SelectedCameraName, false);
                    var skeletonVideo = GetVideoPath(SignName, SelectedCameraName, true);
                    await Task.Yield();
                    await jointsRecognizerService.CreateSkeletonVideo(sourceVideo, skeletonVideo);
                    if (!TryLoadVideo(targetVideoName))
                    {
                        await dialogService.DisplayAlert("錯誤", $"無法載入影片\r\n{targetVideoName}", "確認");
                    }
                }
                catch (Exception ex)
                {
                    await dialogService.DisplayAlert("錯誤", ex.Message, "確認");
                }
            }
            else
            {
                await dialogService.DisplayAlert("錯誤", $"無法載入影片\r\n{targetVideoName}", "確認");
            }
        }
    }

    private string GetVideoPath(string signName, string cameraName, bool isSkeleton)
    {
        var userFolder = preferencesService.UsersFolder;
        var userName = preferencesService.UserName;

        return Path.Combine(
            userFolder,
            userName,
            isSkeleton ? "Skeleton" : "Source",
            $"{signName}_{cameraName}.mp4");
    }

    private bool TryLoadVideo(string videoPath)
    {
        if (File.Exists(videoPath))
        {
            MediaSource = videoPath;
            return true;
        }
        return false;
    }
}
