using System.ComponentModel;

namespace SignLanguageRecorder.Controls;

public class Icon : Label
{
    public static readonly BindableProperty SymbolProperty = BindableProperty.Create(
    nameof(Symbol),
    typeof(IconSymbol),
    typeof(Icon),
    default(IconSymbol),
    propertyChanged: OnSymbolPropertyChanged
    );

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColor),
        typeof(Color),
        typeof(Icon),
        default(Color),
        propertyChanged: OnIconColorPropertyChanged
        );

    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSize),
        typeof(IconSize),
        typeof(Icon),
        default(IconSize),
        propertyChanged: OnIconSizePropertyChanged
        );

    private static void OnSymbolPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Icon)bindable;
        control.Symbol = (IconSymbol)newValue;
    }

    private static void OnIconColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Icon)bindable;
        control.IconColor = (Color)newValue;
    }

    private static void OnIconSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Icon)bindable;
        control.IconSize = (IconSize)newValue;
    }

    internal const string UsingFontFamilyName = "MaterialDesignIcon";

    private IconSymbol symbol;

    public IconSymbol Symbol
    {
        get => symbol;
        set
        {
            if (symbol == value)
                return;
            symbol = value;
            Text = value.GetGlyphCode();
            OnPropertyChanged();
            OnPropertyChanged(nameof(Text));
        }
    }

    public Color IconColor
    {
        get => TextColor;
        set
        {
            if (TextColor == value)
                return;
            TextColor = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TextColor));
        }
    }

    public IconSize IconSize
    {
        get => (IconSize)(int)FontSize;
        set
        {
            var intValue = (int)value;
            if (FontSize == intValue)
                return;
            FontSize = intValue;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FontSize));
        }
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
    public static void FixUnpackAppMissingFontFamily(string fontFile, string fontName)
    {
        var fontFamily = $"ms-appx:///{fontFile}#{fontName}";
#if WINDOWS
        Microsoft.Maui.Handlers.LabelHandler.Mapper.AppendToMapping("FontFamily", (handler, element) =>
        {
            if (element.Font.Family == UsingFontFamilyName)
            {
                if (element is Icon icon)
                {
                    handler.PlatformView.FontFamily = new Microsoft.UI.Xaml.Media.FontFamily(fontFamily);
                    icon.FontFamily = fontFamily;
                }
            }
        });
#endif
    }
}
