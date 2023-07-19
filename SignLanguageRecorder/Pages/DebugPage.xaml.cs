namespace SignLanguageRecorder.Pages;

public partial class DebugPage : ContentPage,
    IWithViewModel<DebugPageViewModel>,
    DebugPageViewModel.IRequirement
{
    public DebugPageViewModel ViewModel => BindingContext as DebugPageViewModel;

    public DebugPage()
    {
        InitializeComponent();
        BindingContext = new DebugPageViewModel(this);
    }
}