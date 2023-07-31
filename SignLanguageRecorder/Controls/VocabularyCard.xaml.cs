namespace SignLanguageRecorder.Controls;

public partial class VocabularyCard : ContentView,
      IWithViewModel<VocabularyCardViewModel>
{
    public VocabularyCardViewModel ViewModel { get; }

    public VocabularyCard()
    {
        InitializeComponent();
        ViewModel = new VocabularyCardViewModel();
    }
}