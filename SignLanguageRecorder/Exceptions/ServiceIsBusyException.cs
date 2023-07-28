namespace SignLanguageRecorder.Exceptions;

public class ServiceIsBusyException : Exception
{
    public ServiceIsBusyException() : base() { }

    public ServiceIsBusyException(string message) : base(message) { }

    public ServiceIsBusyException(string message, Exception innerException)
        : base(message, innerException) { }
}
