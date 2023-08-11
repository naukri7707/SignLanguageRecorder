using System.Diagnostics;
using System.Text;

namespace SignLanguageRecorder.ViewModels;

public partial class DebugPageViewModel : ObservableObject
{
    private readonly IRequirement requirement;

    public DebugPageViewModel(IRequirement requirement)
    {
        this.requirement = requirement;
    }

    [RelayCommand]
    public async Task TestDemoVideoIntegrity()
    {
        var voc = Dependency.Inject<VocabularyService>();
        var pre = Dependency.Inject<PreferencesService>();
        var infos = await voc.GetVocabularyInfos();
        var list = new List<string>();
        var sb = new StringBuilder();
        foreach (var info in infos)
        {
            var path = Path.Combine(pre.DemoFolder, $"{info.Name}.mp4");
            if (!File.Exists(path))
            {
                list.Add(info.Name);
                sb.Append(path);
            }
        }
        var b = list.ToArray();

        Debug.WriteLine(sb.ToString());
    }

    public interface IRequirement
    {
    }
}