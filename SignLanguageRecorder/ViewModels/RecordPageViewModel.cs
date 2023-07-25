using LiteDB;
using System.ComponentModel;

namespace SignLanguageRecorder.ViewModels;

public partial class RecordPageViewModel : ObservableObject
{
    public interface IRequirement
    {
        public Grid RecorderContainer { get; }
    }

    private readonly IRequirement requirement;

    private readonly DatabaseService databaseService;

    private readonly RecordService recordService;

    [ObservableProperty]
    private bool isRecording;

    public ObservableCollection<VocabularyInfo> VocabularyInfos { get; } = new();

    [ObservableProperty]
    private VocabularyInfo selectedVocabularyInfo;

    public int ActivedRecorderCount => recordService.ActivedRecorderCount;

    public RecordPageViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<DatabaseService>(),
        Dependency.Inject<RecordService>()
        )
    { }

    public RecordPageViewModel(IRequirement requirement, DatabaseService databaseService, RecordService recordService)
    {
        this.requirement = requirement;
        this.databaseService = databaseService;
        this.recordService = recordService;

        foreach (var recorder in recordService.Recorders)
        {
            requirement.RecorderContainer.Children.Add(recorder);
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

    [RelayCommand]
    public void RecordButton_Clicked()
    {
        if (recordService.IsRecording)
        {
            var name = SelectedVocabularyInfo.Name;
            recordService.StartRecording(name);
        }
        else
        {
            recordService.StopRecording();
        }
    }

    public bool TrySetActivedRecorderCount(int recorderCount)
    {
        try
        {
            recordService.ActivedRecorderCount = recorderCount;
            return true;
        }
        catch
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
        recordService.SaveLayout(layoutName);
    }

    public bool LoadLayout(string layoutName)
    {
        return recordService.LoadLayout(layoutName);
    }


    public bool DeleteLayout(string layoutName)
    {
        return recordService.DeleteLayout(layoutName);
    }

    public string[] GetLayoutNames()
    {
        return recordService.GetLayoutNames();
    }
}

