using Python.Runtime;
using System.Diagnostics;

namespace SignLanguageRecorder.Services
{
    public class PythonService : IDisposable
    {
        private readonly PreferencesService preferencesService;

        public bool IsBusy { get; private set; }

        public PythonService(PreferencesService preferencesService)
        {
            this.preferencesService = preferencesService;
            _ = InitializePython();
        }


        private async Task InitializePython()
        {
            if (IsBusy)
            {
                throw new ServiceBusyException();
            }

            await Task.Yield();
            IsBusy = true;
            var pythonDll = Path.Combine(preferencesService.PythonFolder, "Python311", "python311.dll");
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll, EnvironmentVariableTarget.Process);
            PythonEngine.Initialize();
            IsBusy = false;
        }

        public async Task<(PyModule module, PyDict members)> CreateScope(string scopeName, string scriptFile)
        {
            if (IsBusy)
            {
                throw new ServiceBusyException();
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
}
