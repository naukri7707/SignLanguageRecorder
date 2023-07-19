using Camera.MAUI;

namespace SignLanguageRecorder.ViewModels;

public partial class RecorderViewModel : ObservableObject
{
    public interface IRequirement
    {
        public CameraView CameraView { get; }
    }

    private const string DISABLED = "Disabled";

    private const string UNKNOW = "Unknow";

    private const string DISABLED_NAME = "關閉";

    private readonly IRequirement requirement;

    private readonly PreferencesService preferencesService;

    [ObservableProperty]
    private string recorderName;

    public string SelectedCameraId { get; private set; }

    public string SelectedMicrophoneId { get; private set; }

    public string[] CameraNameOptions => new[] { DISABLED_NAME }.Concat(requirement.CameraView.Cameras.Select(it => it.Name)).ToArray();

    public string[] MicrophoneNameOptions => new[] { DISABLED_NAME }.Concat(requirement.CameraView.Microphones.Select(it => it.Name)).ToArray();

    public RecorderViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<PreferencesService>()
        )
    { }

    public RecorderViewModel(IRequirement requirement, PreferencesService preferencesService)
    {
        this.requirement = requirement;
        this.preferencesService = preferencesService;
    }

    public void EnableQRCodeScanner(CameraView cameraView)
    {
        cameraView.BarCodeOptions = new Camera.MAUI.ZXingHelper.BarcodeDecodeOptions
        {
            AutoRotate = true,
            PossibleFormats = { ZXing.BarcodeFormat.QR_CODE },
            ReadMultipleCodes = false,
            TryHarder = true,
            TryInverted = true
        };
        cameraView.BarCodeDetectionFrameRate = 10;
        cameraView.BarCodeDetectionMaxThreads = 5;
        cameraView.ControlBarcodeResultDuplicate = true;
        cameraView.BarCodeDetectionEnabled = true;
    }

    public async Task<CameraResult> StartRecordAsync(string videoName)
    {
        var cameraView = requirement.CameraView;
        // Todo 用專用的 Service 繪製
        if (cameraView.Camera == null)
        {
            await Application.Current.MainPage.DisplayAlert("錯誤", $"{RecorderName}沒有指定攝影機", "OK");
            return CameraResult.NoCameraSelected;
        }

        if (cameraView.Microphone == null)
        {
            await Application.Current.MainPage.DisplayAlert("錯誤", $"{RecorderName}沒有指定麥克風", "OK");
            return CameraResult.NoMicrophoneSelected;
        }

        var userFolder = preferencesService.UsersFolder;
        var userName = preferencesService.UserName;

        var folderPath = Path.Combine(userFolder, userName, "Source", RecorderName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, $"{videoName}.mp4");

        var result = await cameraView.StartRecordingAsync(filePath);
        return result;
    }

    public async Task<CameraResult> StopRecordAsync()
    {
        var cameraView = requirement.CameraView;
        return await cameraView.StopRecordingAsync();
    }

    public RecorderInfo GetInfo()
    {
        return new RecorderInfo(RecorderName, SelectedCameraId, SelectedMicrophoneId);
    }

    public void SetInfo(RecorderInfo info)
    {
        // Todo 沒有找到裝置時警告
        RecorderName = info.Name;
        SelectedCameraId = info.CameraId;
        SelectedMicrophoneId = info.MicrophoneId;
        ResetCameraView();
    }

    private void ResetCameraView()
    {
        var cameraView = requirement.CameraView;

        cameraView.Camera = cameraView.Cameras.FirstOrDefault(it => it.DeviceId == SelectedCameraId);
        cameraView.Microphone = cameraView.Microphones.FirstOrDefault(it => it.DeviceId == SelectedMicrophoneId);
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // stop camera to fix samsung devices bug
            await cameraView.StopCameraAsync();
            var result = await cameraView.StartCameraAsync();
        });
    }

    public string CameraIndexToId(int index)
    {
        var cameras = requirement.CameraView.Cameras;
        if (index >= 0 && index < cameras.Count)
        {
            return requirement.CameraView.Cameras[index].DeviceId;
        }
        else if (index == -1)
        {
            return DISABLED;
        }
        else
        {
            return UNKNOW;
        }
    }

    public string CameraIdToName(string id)
    {
        return id switch
        {
            DISABLED => DISABLED_NAME,
            _ => requirement.CameraView.Cameras.FirstOrDefault(it => it.DeviceId == id)?.Name ?? DISABLED_NAME
        };
    }

    public string MicrophoneIndexToId(int index)
    {
        var microphones = requirement.CameraView.Microphones;
        if (index >= 0 && index < microphones.Count)
        {
            return requirement.CameraView.Microphones[index].DeviceId;
        }
        else if (index == -1)
        {
            return DISABLED;
        }
        else
        {
            return UNKNOW;
        }
    }

    public string MicrophoneIdToName(string id)
    {
        return id switch
        {
            DISABLED => DISABLED_NAME,
            _ => requirement.CameraView.Microphones.FirstOrDefault(it => it.DeviceId == id)?.Name ?? DISABLED_NAME
        };
    }
}