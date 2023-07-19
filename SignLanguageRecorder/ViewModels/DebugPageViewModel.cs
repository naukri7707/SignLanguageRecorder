namespace SignLanguageRecorder.ViewModels;

public partial class DebugPageViewModel : ObservableObject
{
    public interface IRequirement
    {
    }

    private readonly IRequirement requirement;

    public DebugPageViewModel(IRequirement requirement)
    {
        this.requirement = requirement;
    }
}

