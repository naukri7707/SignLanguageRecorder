namespace SignLanguageRecorder.ViewModels;

public partial class RecordPageViewModel : ObservableObject
{
    public interface IRequirement
    {
        public Grid RecorderContainer { get; }

        public Recorder[] Recorders { get; }
    }

    private readonly IRequirement requirement;

    private readonly DatabaseService databaseService;

    private readonly RecorderLayoutService recorderLayoutService;

    [ObservableProperty]
    private bool isRecording;

    public int CamCount { get; private set; } = 1;

    public ObservableCollection<SignLanguageInfo> GestureTasks { get; } = new();

    [ObservableProperty]
    private SignLanguageInfo selectedGestureTask;

    public RecordPageViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<DatabaseService>(),
        Dependency.Inject<RecorderLayoutService>()
        )
    { }

    public RecordPageViewModel(IRequirement requirement, DatabaseService databaseService, RecorderLayoutService recorderLayoutService)
    {
        this.requirement = requirement;
        this.databaseService = databaseService;
        this.recorderLayoutService = recorderLayoutService;
    }

    public async void UpdateGestureTasks()
    {
        await Task.Yield();
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<SignLanguageInfo>();

            GestureTasks.Clear();

            foreach (var task in collection.FindAll())
            {
                GestureTasks.Add(task);
            }
        }
    }

    [RelayCommand]
    public async void RecordButton_Clicked()
    {
        await Task.Yield();
        var recorders = requirement.Recorders;
        var taskName = SelectedGestureTask.Name;
        for (int i = 0; i < CamCount; i++)
        {
            var recorder = recorders[i];
            var fileName = $"{taskName}_{recorder.ViewModel.RecorderName}";

            if (IsRecording)
            {
                recorder.StopRecord();
            }
            else
            {
                recorder.StartRecord();
            }
        }
    }

    public bool TrySetCamCount(int camCount)
    {
        if (camCount > 0 && camCount <= 16)
        {
            CamCount = camCount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OpenSavedataFolder()
    {
        string folderPath = FileSystem.AppDataDirectory;
        // Todo : 支援多平台
        System.Diagnostics.Process.Start("explorer.exe", folderPath);
    }

    public void SaveLayout(string layoutName)
    {
        var infos = requirement.Recorders
            .Select(recorder => recorder.ViewModel.GetInfo())
            .Take(CamCount)
            .ToArray();
        recorderLayoutService.SaveLayout(layoutName, infos);
    }

    public bool LoadLayout(string layoutName)
    {
        var layout = recorderLayoutService.LoadLayout(layoutName);

        if (layout == null)
        {
            return false;
        }

        CamCount = layout.Infos.Length;
        for (var i = 0; i < CamCount; i++)
        {
            var info = layout.Infos[i];
            requirement.Recorders[i].ViewModel.SetInfo(info);
        }
        return true;
    }

    public bool DeleteLayout(string layoutName)
    {
        return recorderLayoutService.DeleteLayout(layoutName);
    }

    public string[] GetLayoutNames()
    {
        return recorderLayoutService.GetLayoutNames();
    }
}

