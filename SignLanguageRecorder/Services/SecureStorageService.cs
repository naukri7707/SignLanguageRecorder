namespace SignLanguageRecorder.Services;

public class SecureStorageService
{
    public Task<string> Get(string key)
    {
        return SecureStorage.Default.GetAsync(key);
    }

    public Task Set(string key, string value)
    {
        return SecureStorage.Default.SetAsync(key, value);
    }

    public Task Remove<T>(string key)
    {
        SecureStorage.Default.Remove(key);
        return Task.CompletedTask;
    }
}
