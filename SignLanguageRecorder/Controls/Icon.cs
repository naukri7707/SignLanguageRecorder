using SignLanguageRecorder.Utilities;
using System.ComponentModel;

namespace SignLanguageRecorder.Controls;

public class Icon : Label
{
    internal const string UsingFontFamilyName = "MaterialDesignIcon";

    private IconSymbol symbol;

    public IconSymbol Symbol
    {
        get => symbol;
        set
        {
            symbol = value;
            Text = value.GetGlyphCode();
        }
    }

    public Color IconColor
    {
        get => TextColor;
        set => TextColor = value;
    }

    public IconSize IconSize
    {
        get => (IconSize)(int)FontSize;
        set => FontSize = (int)value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Color TextColor
    {
        get => base.TextColor;
        set => base.TextColor = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new double FontSize
    {
        get => base.FontSize;
        set => base.FontSize = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new string FontFamily
    {
        get => base.FontFamily;
        set => base.FontFamily = value;
    }

    public Icon()
    {
        FontFamily = UsingFontFamilyName;
        HorizontalOptions = LayoutOptions.Center;
        VerticalOptions = LayoutOptions.Center;
        IconSize = IconSize.Small;
    }

    /// <summary>
    /// 處理 Unpack app icon font 遺失問題
    /// </summary>
    /// <param name="fontFile">字形檔在專案建置後中的路徑</param>
    /// <param name="fontName">字形檔確切名稱 (在字形檔案右鍵 -> 預覽中可以查看)</param>
    public static void FixUnpackAppMissingFont(string fontFile, string fontName)
    {
        var fontFamily = $"ms-appx:///{fontFile}#{fontName}";
#if WINDOWS
        Microsoft.Maui.Handlers.LabelHandler.Mapper.AppendToMapping("FontFamily", (handler, element) =>
        {
            if (element.Font.Family == UsingFontFamilyName)
            {
                if (element is Icon icon)
                {
                    icon.FontFamily = fontFamily;
                }
            }
        });
#endif
    }
}
