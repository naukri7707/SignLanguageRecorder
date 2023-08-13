using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SignLanguageRecorder.ViewModels;

public partial class VocabularyCardViewModel : INotifyPropertyChanged
{
    private readonly PreferencesService preferencesService;

    private bool isCompleted;

    public VocabularyCardViewModel() : this(
        Dependency.Inject<PreferencesService>()
                                           )
    { }

    public VocabularyCardViewModel(PreferencesService preferencesService)
    {
        this.preferencesService = preferencesService;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public Color Color => IsCompleted ? Colors.Green : Application.Current.RequestedTheme == AppTheme.Dark
        ? Colors.White
        : Colors.Black;

    public bool IsCompleted
    {
        get => isCompleted;
        private set
        {
            if (isCompleted == value)
                return;
            isCompleted = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotCompleted));
            OnPropertyChanged(nameof(Color));
            OnPropertyChanged(nameof(Symbol));
        }
    }

    public bool IsNotCompleted => !IsCompleted;

    public string Name { get; set; }

    public IconSymbol Symbol => IsCompleted ? IconSymbol.CheckBold : IconSymbol.CircleMedium;

    public void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void UpdateCompletion()
    {
        var userFolder = preferencesService.UsersFolder;
        var userName = preferencesService.UserName;
        var folderPath = Path.Combine(userFolder, userName, "Source");

        if (Directory.Exists(folderPath))
        {
            var files = Directory.GetFiles(folderPath, $"{Name}_*.mp4");
            IsCompleted = files.Length > 0;
        }
    }
}
