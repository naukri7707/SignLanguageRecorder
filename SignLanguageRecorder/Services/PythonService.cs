using Python.Runtime;

namespace SignLanguageRecorder.Services
{
    public class PythonService : IDisposable
    {
        public PythonService()
        {
            string pythonDll = @"..\..\Python311\python311.dll";

            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll, EnvironmentVariableTarget.Process);
            PythonEngine.Initialize();
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
        }
    }
}
