using CommunityToolkit.Maui.Storage;
using System.Threading;

namespace SignLanguageRecorder.ViewModels;

public partial class PreferencesPageViewModel : ObservableObject
{
    // Todo: 用 Entry+Picker 製作成 Control 後改用 DataBinding 處理資料
    public interface IRequirement
    {
        Entry DataFolderEntry { get; }
    }

    private readonly IRequirement requirement;

    private readonly PreferencesService preferencesService;

    public PreferencesPageViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<PreferencesService>()
        )
    { }

    public PreferencesPageViewModel(IRequirement requirement, PreferencesService preferencesService)
    {
        this.requirement = requirement;
        this.preferencesService = preferencesService;
        //
        requirement.DataFolderEntry.Text = preferencesService.DataFolder;
    }

    [RelayCommand]
    public async void OpenDataFolderPicker(InputView input)
    {
        var ct = new CancellationToken();
        var result = await FolderPicker.Default.PickAsync(ct);
        if (result.IsSuccessful)
        {
            input.Text = result.Folder.Path;
        }
    }

    [RelayCommand]
    public void Save()
    {
        preferencesService.DataFolder = requirement.DataFolderEntry.Text;
    }
}
