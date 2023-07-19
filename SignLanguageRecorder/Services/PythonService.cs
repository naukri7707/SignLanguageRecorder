using Python.Runtime;

namespace SignLanguageRecorder.Services
{
    public class PythonService : IDisposable
    {
        private readonly PreferencesService preferencesService;

        public PythonService(PreferencesService preferencesService)
        {
            var pythonDll = Path.Combine(preferencesService.PythonFolder, "Python311", "python311.dll");

            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll, EnvironmentVariableTarget.Process);
            PythonEngine.Initialize();
            this.preferencesService = preferencesService;
        }

        public PyModule CreateScope(string scopeName, string scriptFile, out PyDict members)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope(scopeName);
                var code = File.ReadAllText(scriptFile);
                members = new PyDict();
                return scope.Exec(code, members);
            }
        }

        public void Dispose()
        {
            PythonEngine.Shutdown();
            GC.SuppressFinalize(this);
        }
    }
}
