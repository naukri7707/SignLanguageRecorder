using Camera.MAUI;

namespace SignLanguageRecorder.ViewModels;

public partial class RecorderViewModel : ObservableObject
{
    public interface IRequirement
    {
        public CameraView CameraView { get; }
    }

    private readonly IRequirement requirement;

    private const string DISABLED = "Disabled";
    private const string UNKNOW = "Unknow";

    private const string DISABLED_NAME = "關閉";

    [ObservableProperty]
    private string recorderName;

    public string SelectedCameraId { get; private set; }

    public string SelectedMicrophoneId { get; private set; }

    public string[] CameraNameOptions => new[] { DISABLED_NAME }.Concat(requirement.CameraView.Cameras.Select(it => it.Name)).ToArray();

    public string[] MicrophoneNameOptions => new[] { DISABLED_NAME }.Concat(requirement.CameraView.Microphones.Select(it => it.Name)).ToArray();

    public RecorderViewModel(IRequirement requirement)
    {
        this.requirement = requirement;
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

    public async Task<CameraResult> StartRecordAsync(CameraView cameraView)
    {
        var fullFileName = $"{RecorderName}_{DateTime.Now:hhmmss}.mp4";
        cameraView.Camera ??= cameraView.Cameras.First();
        cameraView.Microphone ??= cameraView.Microphones.First();
        var filePath = Path.Combine(FileSystem.Current.CacheDirectory, fullFileName);
        return await cameraView.StartRecordingAsync(filePath);
    }

    public async Task<CameraResult> StopRecordAsync(CameraView cameraView)
    {
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