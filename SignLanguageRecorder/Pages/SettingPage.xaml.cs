namespace SignLanguageRecorder.Pages;

public partial class SettingPage : ContentPage,
    IWithViewModel<SettingPageViewModel>,
    SettingPageViewModel.IRequirement
{
    public SettingPageViewModel ViewModel => BindingContext as SettingPageViewModel;

    public SettingPage()
    {
        InitializeComponent();
        BindingContext = new SettingPageViewModel(this);
    }
}