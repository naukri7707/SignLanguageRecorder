namespace SignLanguageRecorder.ViewModels;

public partial class RecordPageViewModel : ObservableObject
{
    public interface IRequirement
    {
        public Grid RecorderContainer { get; }
    }

    private readonly IRequirement requirement;

    private readonly PreferencesService preferencesService;

    private readonly DialogService dialogService;

    private readonly VocabularyService vocabularyService;

    private readonly RecordService recordService;

    [ObservableProperty]
    private bool isRecording;

    public ObservableCollection<VocabularyCardViewModel> VocabularyCards { get; } = new();

    [ObservableProperty]
    private VocabularyCardViewModel selectedVocabularyCard;

    public event Action LayoutChanged = () => { };

    public int ActivedRecorderCount => recordService.ActivedRecorderCount;

    public RecordPageViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<PreferencesService>(),
        Dependency.Inject<DialogService>(),
        Dependency.Inject<VocabularyService>(),
        Dependency.Inject<RecordService>()
        )
    { }

    public RecordPageViewModel(IRequirement requirement, PreferencesService preferencesService, DialogService dialogService, VocabularyService vocabularyService, RecordService recordService)
    {
        this.requirement = requirement;
        this.preferencesService = preferencesService;
        this.dialogService = dialogService;
        this.vocabularyService = vocabularyService;
        this.recordService = recordService;

        foreach (var recorder in recordService.Recorders)
        {
            requirement.RecorderContainer.Children.Add(recorder);
        }
        recordService.OnRecordStateChanged += newState =>
        {
            IsRecording = newState;
        };
    }

    public async Task GetVocabularies()
    {
        await Task.Yield();

        var vocabularyInfos = await vocabularyService.GetVocabularyInfos();
        VocabularyCards.Clear();
        foreach (var info in vocabularyInfos)
        {
            var card = new VocabularyCardViewModel
            {
                Name = info.Name,
            };
            card.UpdateCompletion();
            VocabularyCards.Add(card);
        }

        SelectedVocabularyCard = VocabularyCards.FirstOrDefault();
    }
    
    public void Record()
    {
        var name = SelectedVocabularyCard.Name;
        recordService.StartRecording(name);
    }

    public void Stop()
    {
        SelectedVocabularyCard.UpdateCompletion();
        recordService.StopRecording();
    }

    [RelayCommand]
    private async void MenuButton_Tapped()
    {
        Task<string> DisplayLayoutPicker()
        {
            var layouts = recordService.GetLayoutNames();
            return dialogService.DisplayActionSheet("選擇佈局", null, null, layouts);
        }

        var result = await dialogService.DisplayActionSheet("選擇動作", null, null, "設定攝影機數量", "載入佈局", "儲存佈局", "刪除佈局", "開啟儲存位置");
        switch (result)
        {
            case "設定攝影機數量":
                var newCamCountText = await dialogService.DisplayPromptAsync(
                    "攝影機數量", null, "確定", "取消", "攝影機數量", 2,
                    Keyboard.Numeric, recordService.ActivedRecorderCount.ToString()
                    );
                // 取消
                if (newCamCountText == null)
                {
                    break;
                }
                // 可以轉換成數字
                else if (int.TryParse(newCamCountText, out var newRecorderCount))
                {
                    if (TrySetActivedRecorderCount(newRecorderCount))
                    {
                        LayoutChanged.Invoke();
                    }
                    else
                    {
                        await dialogService.DisplayAlert("錯誤", "攝影機數量必須在1~16之間", "確定");
                    }
                }
                // 不能轉換為數字時
                else
                {
                    await dialogService.DisplayAlert("錯誤", "攝影機數量必須是正整數", "確定");
                }

                break;
            case "載入佈局":
                var targetLoadLayout = await DisplayLayoutPicker();
                // 沒有取消時
                if (targetLoadLayout != null)
                {
                    if (LoadLayout(targetLoadLayout))
                    {
                        LayoutChanged.Invoke();
                    }
                }
                break;
            case "儲存佈局":
                var layoutName = await dialogService.DisplayPromptAsync("儲存佈局", "佈局名稱", "確定", "取消", "佈局名稱", -1, Keyboard.Text);

                // 沒有取消時
                if (layoutName != null)
                {
                    recordService.SaveLayout(layoutName);
                }
                break;
            case "刪除佈局":
                var targetDeleteLayout = await DisplayLayoutPicker();
                // 沒有取消時
                if (targetDeleteLayout != null)
                {
                    if (recordService.DeleteLayout(targetDeleteLayout))
                    {
                        await dialogService.DisplayAlert("完成", $"{targetDeleteLayout} 已刪除", "確定");
                    }
                }
                break;
            case "開啟儲存位置":
                OpenSavedataFolder();
                break;
            default:
                break;
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
}

