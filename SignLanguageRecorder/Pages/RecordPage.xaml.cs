using CommunityToolkit.Maui.Views;

namespace SignLanguageRecorder.Pages;

public partial class RecordPage : ContentPage,
    IWithViewModel<RecordPageViewModel>,
    RecordPageViewModel.IRequirement
{
    public RecordPageViewModel ViewModel => BindingContext as RecordPageViewModel;

    public RecordPage()
    {
        InitializeComponent();
        BindingContext = new RecordPageViewModel(this);
        ViewModel.LayoutChanged += RefreshRecorders;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.GetVocabularies();
    }

    private async void WatchReplayButton_Clicked(object sender, EventArgs e)
    {
        var videoName = ViewModel.SelectedVocabularyCard.Name;
        var replayPopup = new ReplayPopup(videoName);
        var result = await this.ShowPopupAsync(replayPopup);
        ViewModel.SelectedVocabularyCard.UpdateCompletion();
    }

    private async void WatchDemoButton_Clicked(object sender, EventArgs e)
    {
        var demoPopup = new MediaPlayerPopup();
        var videoName = ViewModel.SelectedVocabularyCard.Name;
        demoPopup.ViewModel.LoadDemo(videoName);
        var result = await this.ShowPopupAsync(demoPopup);
    }

    private async void RecordButton_Clicked(object sender, EventArgs e)
    {
        if (ViewModel.IsRecording)
        {
            ViewModel.Stop();
        }
        else
        {
            var countDownPopup = new CountDownPopup(3);
            var result = await this.ShowPopupAsync(countDownPopup);
            ViewModel.Record();
        }
    }

    private void RecorderContainer_SizeChanged(object sender, EventArgs e)
    {
        RefreshRecorders();
    }

    private int CalculateColumnCount(int camCount)
    {
        var sqrt = Math.Sqrt(camCount);
        var result = (int)Math.Ceiling(sqrt);
        return result;
    }

    private int CalculateRowCount(int camCount)
    {
        var columnCount = CalculateColumnCount(camCount);

        return (columnCount != 0) switch
        {
            true => (camCount / columnCount) + (camCount % columnCount != 0 ? 1 : 0),
            false => 0
        };
    }

    private ColumnDefinitionCollection CalculateColumnDefinitionsOfRecorderContainer(int columnCount)
    {
        var def = Enumerable
            .Range(0, columnCount)
            .Select(it => new ColumnDefinition(GridLength.Star))
            .ToArray();
        return new ColumnDefinitionCollection(def);
    }

    private RowDefinitionCollection CalculateRowDefinitionsOfRecorderContainer(int rowCount)
    {
        var def = Enumerable
            .Range(0, rowCount)
            .Select(it => new RowDefinition(GridLength.Star))
            .ToArray();
        return new RowDefinitionCollection(def);
    }

    public void RefreshRecorders()
    {
        var recorderContainer = RecorderContainer;
        var recorders = recorderContainer.Children.OfType<Recorder>().ToArray();
        var recorderCount = ViewModel.ActivedRecorderCount;
        var columnCount = CalculateColumnCount(recorderCount);
        var rowCount = CalculateRowCount(recorderCount);
        recorderContainer.ColumnDefinitions = CalculateColumnDefinitionsOfRecorderContainer(columnCount);
        recorderContainer.RowDefinitions = CalculateRowDefinitionsOfRecorderContainer(rowCount);

        if (recorderCount == 0)
        {
            for (int i = 0; i < 16; i++)
            {
                var ve = recorders[i];
                ve.IsEnabled = false;
                ve.IsVisible = false;
            }
            return;
        }

        var camWidth = recorderContainer.Width / columnCount;
        var camHeight = recorderContainer.Height / rowCount;

        var cellCount = columnCount * rowCount;
        var emptyCount = cellCount - recorderCount;
        var flexRowFirstIndex = recorderCount % columnCount == 0
            ? int.MaxValue // No flex row
            : cellCount - columnCount;
        var flexRowTranslationX = emptyCount * camWidth / 2;

        for (int i = 0; i < 16; i++)
        {
            var ve = recorders[i];
            if (i < recorderCount)
            {
                // 設定 view 欄位
                var c = i % columnCount;
                var r = i / columnCount;
                recorderContainer.SetColumn(ve, c);
                recorderContainer.SetRow(ve, r);
                // 設定預覽畫面大小
                ve.WidthRequest = camWidth;
                ve.HeightRequest = camHeight;
                // 調整數量不足的 row 中 view 的位置
                if (i >= flexRowFirstIndex)
                {
                    ve.TranslationX = flexRowTranslationX;
                }
                else
                {
                    ve.TranslationX = 0;
                }
                ve.IsEnabled = true;
                ve.IsVisible = true;
            }
            else
            {
                ve.IsEnabled = false;
                ve.IsVisible = false;
            }
        }
    }

    Grid RecordPageViewModel.IRequirement.RecorderContainer => RecorderContainer;
}