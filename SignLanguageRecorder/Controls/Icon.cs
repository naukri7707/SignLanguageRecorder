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
}
