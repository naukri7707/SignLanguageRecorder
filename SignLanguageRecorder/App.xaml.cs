using System.Diagnostics;

namespace SignLanguageRecorder;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        _ = InitializeServiceAsync();
    }

    private async Task InitializeServiceAsync()
    {
        await Task.Yield();
        var pythonService = Dependency.Inject<PythonService>();
        var jointsRecognizerService = Dependency.Inject<JointsRecognizerService>();

        await pythonService.Initialize();
        Debug.WriteLine("Python Service Initialized");
        await jointsRecognizerService.Initialize();
        Debug.WriteLine("Joints Recognizer Service Initialized");
    }
}
