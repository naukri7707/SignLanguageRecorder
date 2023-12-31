﻿namespace SignLanguageRecorder.Services;

public class PreferencesService
{
    // Todo 將 UserName 獨立到 user Service
    public string UserName
    {
        get => Preferences.Get(nameof(UserName), "");
        set => Preferences.Set(nameof(UserName), value);
    }

    public string DemoFolder
    {
        get => Preferences.Get(nameof(DemoFolder), "");
        set => Preferences.Set(nameof(DemoFolder), value);
    }

    public string UsersFolder
    {
        get => Preferences.Get(nameof(UsersFolder), "");
        set => Preferences.Set(nameof(UsersFolder), value);
    }

    public string PythonFolder
    {
        get => Preferences.Get(nameof(PythonFolder), "");
        set => Preferences.Set(nameof(PythonFolder), value);
    }

    public string UserFolder => Path.Combine(UsersFolder, UserName);

    public PreferencesService()
	{
    }

    public void Clear()
    {
        Preferences.Clear();
    }
}