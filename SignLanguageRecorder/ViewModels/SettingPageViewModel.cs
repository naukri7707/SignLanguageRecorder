using LiteDB;

namespace SignLanguageRecorder.ViewModels;

public partial class SettingPageViewModel : ObservableObject
{
    public interface IRequirement
    {

    }

    private readonly IRequirement requirement;

    private readonly DatabaseService databaseService;

    public ObservableCollection<SignLanguageTabViewModel> GestureTasks { get; } = new();

    public SettingPageViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<DatabaseService>()
        )
    { }

    public SettingPageViewModel(IRequirement requirement, DatabaseService databaseService)
    {
        this.requirement = requirement;
        this.databaseService = databaseService;
        GetAllTask();
    }

    [RelayCommand]
    public async void AddNewTask()
    {
        await Task.Yield();
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var task = new SignLanguageInfo();
            var id = db.GetCollection<SignLanguageInfo>().Insert(task);
            task.Id = id;
            var row = new SignLanguageTabViewModel(task);
            GestureTasks.Add(row);
        }
    }

    [RelayCommand]
    public async void GetAllTask()
    {
        await Task.Yield();
        lock (databaseService)
        {
            if (GestureTasks.Count != 0)
                GestureTasks.Clear();
            using var db = databaseService.GetLiteDatabase();
            var tasks = db.GetCollection<SignLanguageInfo>().FindAll();
            foreach (var task in tasks)
            {
                var row = new SignLanguageTabViewModel(task);
                GestureTasks.Add(row);
            }
        }
    }

    [RelayCommand]
    public async void DeleteSelectedTask()
    {
        await Task.Yield();
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<SignLanguageInfo>();

            var selectedRows = GestureTasks.Where(it => it.IsSelected).ToList();
            foreach (var row in selectedRows)
            {
                collection.Delete(row.Source.Id);
                GestureTasks.Remove(row);
            }
        }
    }

    [RelayCommand]
    public async void UpdateChangedTask()
    {
        await Task.Yield();
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<SignLanguageInfo>();

            var selectedRows = GestureTasks.Where(it => it.IsChanged).ToList();
            foreach (var row in selectedRows)
            {
                collection.Update(row.Value);
                row.ConfirmChanged();
            }
        }
    }
}

