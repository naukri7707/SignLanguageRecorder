using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;

namespace SignLanguageRecorder.Controls;

public partial class ReplayPopup : Popup,
      IWithViewModel<ReplayPopupViewModel>,
      ReplayPopupViewModel.IRequirement
{
    public ReplayPopupViewModel ViewModel => BindingContext as ReplayPopupViewModel;

    public ReplayPopup(string videoName)
    {
        InitializeComponent();
        BindingContext = new ReplayPopupViewModel(this, videoName);
    }

    private void MediaPlayer_PointerExited(object sender, PointerEventArgs e)
    {
        var mediaPlayer = (MediaElement)sender;
        mediaPlayer.ShouldShowPlaybackControls = false;
    }

    private void MediaPlayer_PointerMoved(object sender, PointerEventArgs e)
    {
        var mediaPlayer = (MediaElement)sender;
        var relativePos = e.GetPosition(mediaPlayer);

        mediaPlayer.ShouldShowPlaybackControls
             = relativePos.HasValue
            && IsInsideControlPanel(mediaPlayer, relativePos.Value);
    }

    private void MediaPlayer_Tapped(object sender, TappedEventArgs e)
    {
        // 因為 Action Icon 也會觸發所以直接指定 MediaPlayer
        var mediaPlayer = MediaPlayer;
        var relativePos = e.GetPosition(mediaPlayer);

        if (relativePos.HasValue && !IsInsideControlPanel(mediaPlayer, relativePos.Value))
        {
            if (mediaPlayer.CurrentState == MediaElementState.Paused)
            {
                mediaPlayer.Play();
                ActionIcon.Symbol = IconSymbol.PlayCircle;
                FadeOut(ActionIcon);
            }
            else if (mediaPlayer.CurrentState == MediaElementState.Playing)
            {
                mediaPlayer.Pause();
                ActionIcon.Symbol = IconSymbol.PauseCircle;
                FadeOut(ActionIcon);
            }
        }
    }

    private bool IsInsideControlPanel(MediaElement mediaPlayer, Point point)
    {
        var controlSize = new Size(580, 100);
        var location = new Point
        {
            X = mediaPlayer.Width / 2 - controlSize.Width / 2,
            Y = mediaPlayer.Height - controlSize.Height
        };
        var rect = new Rect(location, Size);
        return rect.Contains(point);
    }

    private void FadeOut(VisualElement ve, uint length = 400)
    {
        ve.Opacity = 1;
        ve.FadeTo(0, length);
        ve.Scale = 1;
        ve.ScaleTo(2, length);
    }

    private void Popup_Closed(object sender, PopupClosedEventArgs e)
    {
        MediaPlayer.Handler?.DisconnectHandler();
    }

    private void FileNamePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewModel.UpdateVideo();
    }

    private void ShowSkeletonCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        ViewModel.UpdateVideo();
    }
}