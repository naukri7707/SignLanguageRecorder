namespace SignLanguageRecorder.Pages;

public partial class PreferencesPage : ContentPage,
    IWithViewModel<PreferencesPageViewModel>,
    PreferencesPageViewModel.IRequirement
{
    public PreferencesPageViewModel ViewModel => BindingContext as PreferencesPageViewModel;

    public PreferencesPage()
    {
        InitializeComponent();
        BindingContext = new PreferencesPageViewModel(this);
    }

    Entry PreferencesPageViewModel.IRequirement.DataFolderEntry => DataFolderEntry;
}