namespace SignLanguageRecorder.Exceptions;

public class ServiceNotInitializedException : Exception
{
    public ServiceNotInitializedException() : base() { }

    public ServiceNotInitializedException(string message) : base(message) { }

    public ServiceNotInitializedException(string message, Exception innerException)
        : base(message, innerException) { }
}
