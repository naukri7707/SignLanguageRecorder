namespace SignLanguageRecorder.Abstrations;

public interface IValueEquals
{
    public bool IsValueEquals(object other);
}

public interface IValueEquals<T> : IValueEquals
{
    public bool IsValueEquals(T other);
}
