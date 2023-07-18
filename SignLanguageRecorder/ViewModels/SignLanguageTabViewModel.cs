using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SignLanguageRecorder.ViewModels;

public class SignLanguageTabViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private SignLanguageInfo _source;

    private SignLanguageInfo _value;

    private bool _isSelected;

    public SignLanguageTabViewModel(SignLanguageInfo info)
    {
        _source = info;
        _value = info.DeepCopy();
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public bool IsChanged => !Source.IsValueEquals(Value);

    public Color IsChangedBackgroundColor => IsChanged ? Color.FromUint(0x3300FFFF) : Colors.Transparent;

    public string Name
    {
        get => _value.Name;
        set
        {
            Value.Name = value;
            OnPropertyChanged();
            CheckIsDataChanged();
        }
    }

    public string DemoVideoSource
    {
        get => _value.DemoVideoSource;
        set
        {
            Value.DemoVideoSource = value;
            OnPropertyChanged();
            CheckIsDataChanged();
        }
    }

    public SignLanguageInfo Source
    {
        get => _source;
        private set
        {
            _source = value;
            OnPropertyChanged();
            CheckIsDataChanged();
        }
    }

    public SignLanguageInfo Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged();
            CheckIsDataChanged();
        }
    }

    public void CheckIsDataChanged()
    {
        OnPropertyChanged(nameof(IsChanged));
        OnPropertyChanged(nameof(IsChangedBackgroundColor));
    }

    public void ConfirmChanged()
    {
        Source = Value.DeepCopy();
    }

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

