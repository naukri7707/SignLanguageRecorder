using System.ComponentModel;

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

    public ObservableCollection<VocabularyInfo> VocabularyInfos { get; } = new();

    [ObservableProperty]
    private VocabularyInfo selectedVocabularyInfo;

    [ObservableProperty]
    private int selectedVocabularySignIndex = -1;

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
        //
        for (int i = 0; i < requirement.Recorders.Length; i++)
        {
            var recorder = requirement.Recorders[i];
            if(string.IsNullOrEmpty(recorder.ViewModel.RecorderName))
            {
                recorder.ViewModel.RecorderName = $"Cam {i}";
            }
        }
    }

    public async void UpdateVocabularies(Action onUpdated = null)
    {
        await Task.Yield();
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<VocabularyInfo>();

            VocabularyInfos.Clear();

            foreach (var task in collection.FindAll())
            {
                VocabularyInfos.Add(task);
            }

            onUpdated?.Invoke();
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(SelectedVocabularyInfo))
        {
           SelectedVocabularySignIndex = 0;
        }
    }

    [RelayCommand]
    public async void RecordButton_Clicked()
    {
        await Task.Yield();
        var recorders = requirement.Recorders;
        for (int i = 0; i < CamCount; i++)
        {
            var recorder = recorders[i];

            if (IsRecording)
            {
                _ = recorder.ViewModel.StopRecordAsync();
            }
            else
            {
                var signIndex = SelectedVocabularySignIndex;
                var videoName = SelectedVocabularyInfo.GetVideoName(signIndex);
                _ = recorder.ViewModel.StartRecordAsync(videoName);
            }
        }
        IsRecording = !IsRecording;
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

