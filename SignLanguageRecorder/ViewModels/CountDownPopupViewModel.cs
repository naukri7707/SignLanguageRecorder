using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace SignLanguageRecorder.ViewModels;

public partial class CountDownPopupViewModel : ObservableObject
{
    public interface IRequirement
    {
    }

    private readonly IRequirement requirement;


    [ObservableProperty]
    public int timePerCount = 500;

    [ObservableProperty]
    public int countDown;

    public event Action OnCompleted = () => { };

    public CountDownPopupViewModel(IRequirement requirement, int countDown)
    {
        this.requirement = requirement;
        CountDown = countDown;
        _ = StartCountDown();
    }

    private async Task StartCountDown()
    {
        while (CountDown > 0)
        {
            await Task.Delay(TimePerCount);
            CountDown--;
        }
        OnCompleted.Invoke();
    }
}
