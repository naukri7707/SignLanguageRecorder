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
        var dialog = Dependency.Inject<DialogService>();
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

        if (list.Count > 0)
        {
            await dialog.DisplayAlert("錯誤", $"找不到以下影片：\r\n{string.Join("\r\n", list)}", "確認");
        }
        else
        {
            await dialog.DisplayAlert("完成", $"驗證成功", "確認");
        }
    }

    public interface IRequirement
    {
    }
}