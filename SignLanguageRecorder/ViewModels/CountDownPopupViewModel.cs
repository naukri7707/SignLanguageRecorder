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
    public int countDown;

    public event Action OnCompleted = () => { };

    public CountDownPopupViewModel(IRequirement requirement, int seconds)
    {
        this.requirement = requirement;
        CountDown = seconds;
        _ = StartCountDown();
    }

    private async Task StartCountDown()
    {
        while (CountDown > 0)
        {
            await Task.Delay(1000);
            CountDown--;
        }
        OnCompleted.Invoke();
    }
}
