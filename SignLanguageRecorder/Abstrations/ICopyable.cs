namespace SignLanguageRecorder.Abstrations;

public interface ICopyable<T>
{
    public T ShallowCopy();

    public T DeepCopy();
}
