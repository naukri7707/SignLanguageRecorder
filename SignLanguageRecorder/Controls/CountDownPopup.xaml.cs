using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;

namespace SignLanguageRecorder.Controls;

public partial class CountDownPopup : Popup,
      IWithViewModel<CountDownPopupViewModel>,
      CountDownPopupViewModel.IRequirement
{
    public CountDownPopupViewModel ViewModel => BindingContext as CountDownPopupViewModel;

    public CountDownPopup(int seconds)
    {
        InitializeComponent();
        BindingContext = new CountDownPopupViewModel(this, seconds);
        ViewModel.OnCompleted += () => Close();
    }
}