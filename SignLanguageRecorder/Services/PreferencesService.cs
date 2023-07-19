namespace SignLanguageRecorder.Services;

public class PreferencesService
{
    public string DataFolder
    {
        get => Preferences.Get(nameof(DataFolder), "");
        set => Preferences.Set(nameof(DataFolder), value);
    }

    public PreferencesService()
	{
    }
}