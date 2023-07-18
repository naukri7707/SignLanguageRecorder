using CommunityToolkit.Maui.Views;

namespace SignLanguageRecorder.Controls;

public partial class RecorderSettingsPopup : Popup,
    IWithViewModel<RecorderViewModel>
{
    public record Response(bool IsCancel, RecorderInfo Info);

    public RecorderViewModel ViewModel => BindingContext as RecorderViewModel;

    public RecorderSettingsPopup(RecorderViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        var selectedCameraName = viewModel.CameraIdToName(viewModel.SelectedCameraId);
        var selectedMicrophoneName = viewModel.MicrophoneIdToName(viewModel.SelectedMicrophoneId);

        RecorderNameEntry.Text = viewModel.RecorderName;
        CameraPicker.SelectedItem = selectedCameraName;
        MicrophonePicker.SelectedItem = selectedMicrophoneName;
    }

    public void SaveAndClose(object sender, EventArgs e)
    {
        var result = CreateResponse(false);
        Close(result);
    }

    public void Close(object sender, EventArgs e)
    {
        var result = CreateResponse(true);
        Close(result);
    }

    private Response CreateResponse(bool isCancel)
    {
        var cameraIndex = CameraPicker.SelectedIndex - 1; // 因為新增了一個關閉選項所以 -1
        var cameraId = ViewModel.CameraIndexToId(cameraIndex);
        var microphoneIndex = MicrophonePicker.SelectedIndex - 1;
        var microphoneId = ViewModel.MicrophoneIndexToId(microphoneIndex);

        var info = new RecorderInfo(
            RecorderNameEntry.Text,
            cameraId,
            microphoneId
            );
        return new Response(isCancel, info);
    }
}