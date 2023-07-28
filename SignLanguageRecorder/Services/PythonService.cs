using Python.Runtime;

namespace SignLanguageRecorder.Services;

public class PythonService : IDisposable
{
    private readonly PreferencesService preferencesService;

    public bool IsBusy { get; private set; }

    public bool IsInitialized { get; private set; }

    public PythonService(PreferencesService preferencesService)
    {
        this.preferencesService = preferencesService;
    }

    public async Task Initialize()
    {
        if (IsBusy)
        {
            throw new ServiceIsBusyException();
        }

        if (IsInitialized)
        {
            return;
        }

        await Task.Yield();
        IsBusy = true;
        var pythonDll = Path.Combine(preferencesService.PythonFolder, "Python311", "python311.dll");
        Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll, EnvironmentVariableTarget.Process);
        PythonEngine.Initialize();
        IsInitialized = true;
        IsBusy = false;
    }

    public async Task<(PyModule module, PyDict members)> CreateScope(string scopeName, string scriptFile)
    {
        if (IsBusy)
        {
            throw new ServiceIsBusyException();
        }

        if(!IsInitialized)
        {
            throw new ServiceNotInitializedException();
        }

        await Task.Yield();
        IsBusy = true;
        using var _ = Py.GIL();
        var scope = Py.CreateScope(scopeName);
        var code = File.ReadAllText(scriptFile);
        var members = new PyDict();
        var module = scope.Exec(code, members);
        IsBusy = false;
        return (module, members);
    }

    public void Dispose()
    {
        PythonEngine.Shutdown();
        GC.SuppressFinalize(this);
    }
}
