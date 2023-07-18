namespace SignLanguageRecorder.Abstrations;

public interface IWithViewModel<out T>
{
    public T ViewModel { get; }
}