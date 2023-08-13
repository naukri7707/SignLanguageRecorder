using Camera.MAUI;
using LiteDB;
using System.Text;
using System.Threading.Tasks;

namespace SignLanguageRecorder.Services;

public class RecordService
{
    private readonly Recorder[] recorders;

    private readonly PreferencesService preferencesService;

    private readonly DatabaseService databaseService;

    private readonly DialogService dialogService;

    public Recorder[] Recorders => recorders;

    private int activedRecorderCount = 1;

    public int ActivedRecorderCount
    {
        get => activedRecorderCount;
        set
        {
            if (value < 1 || value > 16)
            {
                throw new ArgumentException($"{nameof(ActivedRecorderCount)} 必須介於 1 ~ 16 之間");
            }
            activedRecorderCount = value;
        }
    }

    private bool isRecording;

    public bool IsRecording
    {
        get => isRecording;
        private set
        {
            if (value != isRecording)
            {
                isRecording = value;
                OnRecordStateChanged(value);
            }
        }
    }

    public event Action<bool> OnRecordStateChanged = s => { };

    public RecordService() : this(
        Dependency.Inject<PreferencesService>(),
        Dependency.Inject<DatabaseService>(),
        Dependency.Inject<DialogService>()
        )
    { }

    public RecordService(PreferencesService preferencesService, DatabaseService databaseService, DialogService dialogService)
    {
        this.preferencesService = preferencesService;
        this.databaseService = databaseService;
        this.dialogService = dialogService;
        //
        recorders = new Recorder[16];
        for (int i = 0; i < 16; i++)
        {
            var recorder = new Recorder
            {
                IsEnabled = false,
                IsVisible = false,
            };

            if (string.IsNullOrEmpty(recorder.ViewModel.RecorderName))
            {
                recorder.ViewModel.RecorderName = $"Cam {i}";
            }

            recorders[i] = recorder;
        }
    }

    public async Task<bool> PromptForOverwriteIfFileExists(string videoName)
    {
        var recorders = Recorders.Take(ActivedRecorderCount).ToArray();

        var userFolder = preferencesService.UsersFolder;
        var userName = preferencesService.UserName;
        var folderPath = Path.Combine(userFolder, userName, "Source");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 檢查檔案是否已經存在
        var existedList = new StringBuilder();
        foreach (var recorder in recorders)
        {
            var recorderName = recorder.ViewModel.RecorderName;
            var fileName = Path.Combine(folderPath, $"{videoName}_{recorderName}.mp4");
            if (File.Exists(fileName))
            {
                existedList.Append(fileName);
                existedList.Append('\n');
            }
        }
        if (existedList.Length > 0)
        {
            existedList.Length--;
            var overrideFile = await dialogService.DisplayAlert("偵測到檔案已存在，是否覆寫？", existedList.ToString(), "是", "否");

            return overrideFile;
        }
        return true;
    }

    public async void StartRecording(string videoName)
    {
        await Task.Yield();

        var recorders = Recorders.Take(ActivedRecorderCount).ToArray();
        var anyError = false;

        var userFolder = preferencesService.UsersFolder;
        var userName = preferencesService.UserName;
        var folderPath = Path.Combine(userFolder, userName, "Source");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 開始錄製
        var tasks = new List<Task<CameraResult>>();
        foreach (var recorder in recorders)
        {
            var recorderName = recorder.ViewModel.RecorderName;

            var fileName = Path.Combine(folderPath, $"{videoName}_{recorderName}.mp4");
            var task = recorder.ViewModel.StartRecordAsync(fileName);

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        for (var i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];
            var recorder = recorders[i];
            var recorderName = recorder.ViewModel.RecorderName;
            var result = task.Result;

            if (result != CameraResult.Success)
            {
                anyError = true;
                var message = result switch
                {
                    CameraResult.AccessDenied => "拒絕存取",
                    CameraResult.NoCameraSelected => "未選擇相機",
                    CameraResult.AccessError => "存取錯誤",
                    CameraResult.NoVideoFormatsAvailable => "不支援此格式",
                    CameraResult.NotInitiated => "相機未啟動",
                    CameraResult.NoMicrophoneSelected => "未選擇麥克風",
                    CameraResult.ResolutionNotAvailable => "不支援此解析度",
                    _ => throw new NotImplementedException(),
                };

                await dialogService.DisplayAlert(recorderName, message, "確認");
            }
        }

        if (anyError)
        {
            StopRecording();
        }
        else
        {
            IsRecording = true;
        }
    }

    public async void StopRecording()
    {
        await Task.Yield();

        var recorders = Recorders.Take(ActivedRecorderCount);
        var tasks = new List<Task<CameraResult>>();
        foreach (var recorder in recorders)
        {
            var task = recorder.ViewModel.StopRecordAsync();
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);

        IsRecording = false;
    }

    public void SaveLayout(string layoutName)
    {
        var infos = recorders
            .Select(it => it.ViewModel.GetInfo())
            .Take(ActivedRecorderCount)
            .ToArray();

        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<RecorderLayout>();

            var layoutId = collection.FindOne(x => x.Name == layoutName)?.Id ?? ObjectId.NewObjectId();
            var layout = new RecorderLayout
            {
                Name = layoutName,
                Infos = infos
            };
            collection.Upsert(layoutId, layout);
        }
    }

    public bool LoadLayout(string layoutName)
    {
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<RecorderLayout>();
            var layout = collection.FindOne(x => x.Name == layoutName);
            if (layout != null)
            {
                ActivedRecorderCount = layout.Infos.Length;
                for (int i = 0; i < ActivedRecorderCount; i++)
                {
                    var recorder = recorders[i];
                    recorder.ViewModel.SetInfo(layout.Infos[i]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool DeleteLayout(string layoutName)
    {
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<RecorderLayout>();

            var id = collection.FindOne(x => x.Name == layoutName)?.Id;

            if (id == null)
            {
                return false;
            }

            return collection.Delete(id);
        }
    }

    public string[] GetLayoutNames()
    {
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<RecorderLayout>();

            return collection.Query().Select(x => x.Name).ToArray();
        }
    }
}
