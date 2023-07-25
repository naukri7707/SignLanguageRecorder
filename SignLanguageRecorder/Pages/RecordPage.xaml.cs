﻿using CommunityToolkit.Maui.Views;
using System.Runtime.CompilerServices;

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
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.UpdateVocabularies(() =>
        {
            if (ViewModel.SelectedVocabularyInfo == null)
            {
                ViewModel.SelectedVocabularyInfo = ViewModel.VocabularyInfos.FirstOrDefault();
            }
        });
    }

    private async void WatchReplayButton_Clicked(object sender, EventArgs e)
    {
        var videoName = ViewModel.SelectedVocabularyInfo.Name;
        var replayPopup = new ReplayPopup(videoName);
        var result = await this.ShowPopupAsync(replayPopup);
    }

    private async void WatchDemoButton_Clicked(object sender, EventArgs e)
    {
        var demoPopup = new MediaPlayerPopup();
        var videoName = ViewModel.SelectedVocabularyInfo.Name;
        demoPopup.ViewModel.LoadDemo(videoName);
        var result = await this.ShowPopupAsync(demoPopup);
    }

    private void RecorderContainer_SizeChanged(object sender, EventArgs e)
    {
        RefreshRecorders();
    }

    private async void MenuButton_Tapped(object sender, TappedEventArgs e)
    {
        Task<string> DisplayLayoutPicker()
        {
            var layouts = ViewModel.GetLayoutNames();
            return DisplayActionSheet("選擇佈局", null, null, layouts);
        }

        var result = await DisplayActionSheet("選擇動作", null, null, "設定攝影機數量", "載入佈局", "儲存佈局", "刪除佈局", "開啟儲存位置");
        switch (result)
        {
            case "設定攝影機數量":
                var newCamCountText = await DisplayPromptAsync(
                    "攝影機數量", null, "確定", "取消", "攝影機數量", 2,
                    Keyboard.Numeric, ViewModel.ActivedRecorderCount.ToString()
                    );
                // 取消
                if (newCamCountText == null)
                {
                    break;
                }
                // 可以轉換成數字
                else if (int.TryParse(newCamCountText, out var newRecorderCount))
                {
                    if (ViewModel.TrySetActivedRecorderCount(newRecorderCount))
                    {
                        RefreshRecorders();
                    }
                    else
                    {
                        await DisplayAlert("錯誤", "攝影機數量必須在1~16之間", "確定");
                    }
                }
                // 不能轉換為數字時
                else
                {
                    await DisplayAlert("錯誤", "攝影機數量必須是正整數", "確定");
                }

                break;
            case "載入佈局":
                var targetLoadLayout = await DisplayLayoutPicker();
                // 沒有取消時
                if (targetLoadLayout != null)
                {
                    if (ViewModel.LoadLayout(targetLoadLayout))
                    {
                        RefreshRecorders();
                    }
                }
                break;
            case "儲存佈局":
                var layoutName = await DisplayPromptAsync("儲存佈局", "佈局名稱", "確定", "取消", "佈局名稱", -1, Keyboard.Text);

                // 沒有取消時
                if (layoutName != null)
                {
                    ViewModel.SaveLayout(layoutName);
                }
                break;
            case "刪除佈局":
                var targetDeleteLayout = await DisplayLayoutPicker();
                // 沒有取消時
                if (targetDeleteLayout != null)
                {
                    if (ViewModel.DeleteLayout(targetDeleteLayout))
                    {
                        await DisplayAlert("完成", $"{targetDeleteLayout} 已", "確定");
                    }
                }
                break;
            case "開啟儲存位置":
                ViewModel.OpenSavedataFolder();
                break;
            default:
                break;
        }
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