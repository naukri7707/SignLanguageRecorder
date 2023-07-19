namespace SignLanguageRecorder.ViewModels;

public class MediaPlayerPopupViewModel : ObservableObject
{
    public interface IRequirement
    {
    }

    private readonly IRequirement requirement;

    public MediaPlayerPopupViewModel(IRequirement requirement)
    {
        this.requirement = requirement;
    }
}
