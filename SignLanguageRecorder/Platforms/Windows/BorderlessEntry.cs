namespace SignLanguageRecorder.Controls;
using Microsoft.Maui.Platform;

public partial class BorderlessEntry
{
    protected override void OnHandlerChanged()
    {
        var textbox = Handler.PlatformView as MauiPasswordTextBox;
        textbox.Background = Colors.Transparent.ToPlatform();
        // 取消圓角
        textbox.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(0);
        // 取消常態邊框
        textbox.Resources["TextControlBorderThemeThickness"] = new Thickness(0);
        // 如果嘗試取消 Focused 狀態邊框的話，點擊時會拋出 Error
        // textbox.Resources["TextControlBorderThemeThicknessFocused"] = new Thickness(0);
    }
}
