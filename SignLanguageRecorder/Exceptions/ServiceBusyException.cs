namespace SignLanguageRecorder.Exceptions;

public class ServiceBusyException : Exception
{
    public ServiceBusyException() : base() { }

    public ServiceBusyException(string message) : base(message) { }

    public ServiceBusyException(string message, Exception innerException)
        : base(message, innerException) { }
}
