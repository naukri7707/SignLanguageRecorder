using Camera.MAUI;
using CommunityToolkit.Maui.Views;

namespace SignLanguageRecorder.Controls; public partial class Recorder : ContentView,
    IWithViewModel<RecorderViewModel>,
    RecorderViewModel.IRequirement
{
    public RecorderViewModel ViewModel => BindingContext as RecorderViewModel;

    public Recorder()
    {
        InitializeComponent();
        BindingContext = new RecorderViewModel(this);
    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        FadeIn(RecorderNameLabel);
        FadeIn(SettingButton);
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        FadeOut(RecorderNameLabel);
        FadeOut(SettingButton);
    }

    private void FadeIn(View view)
    {
        view.IsEnabled = true;
        view.Scale = 0.8;
        view.ScaleTo(1, 200, Easing.CubicIn);
        view.Opacity = 0;
        view.FadeTo(1, 200, Easing.CubicIn);
    }

    private void FadeOut(View view)
    {
        view.IsEnabled = false;
        view.Scale = 1;
        view.ScaleTo(0.8F, 200, Easing.CubicIn);
        view.Opacity = 1;
        view.FadeTo(0, 200, Easing.CubicIn);
    }

    private async void OnSettingButtonTapped(object sender, TappedEventArgs e)
    {
        var settingsPopup = new RecorderSettingsPopup(ViewModel);

        (bool isCancel, RecorderInfo info) = (RecorderSettingsPopup.Response)await Application.Current.MainPage.ShowPopupAsync(settingsPopup);
        if (!isCancel)
        {
            ViewModel.SetInfo(info);
        }
    }

    public async void StartRecord()
    {
        var cameraView = CameraView;
        await ViewModel.StartRecordAsync(cameraView);
    }

    public async void StopRecord()
    {
        var cameraView = CameraView;
        await ViewModel.StopRecordAsync(cameraView);
    }

    CameraView RecorderViewModel.IRequirement.CameraView => CameraView;
}