﻿using CommunityToolkit.Maui.Storage;
using LiteDB;
using System.Threading;

namespace SignLanguageRecorder.ViewModels;

public partial class PreferencesPageViewModel : ObservableObject
{
    // Todo: 用 Entry+Picker 製作成 Control 後改用 DataBinding 處理資料
    public interface IRequirement
    {
        Entry UserNameEntry { get; }
        Entry UsersFolderEntry { get; }
        Entry DemoFolderEntry { get; }
        Entry PythonFolderEntry { get; }
    }

    private readonly IRequirement requirement;

    private readonly PreferencesService preferencesService;

    private readonly VocabularyService vocabularyService;

    public PreferencesPageViewModel(IRequirement requirement) : this(
        requirement,
        Dependency.Inject<PreferencesService>(),
        Dependency.Inject<VocabularyService>()
        )
    { }

    public PreferencesPageViewModel(IRequirement requirement, PreferencesService preferencesService, VocabularyService vocabularyService)
    {
        this.requirement = requirement;
        this.preferencesService = preferencesService;
        this.vocabularyService = vocabularyService;
        //
        requirement.UserNameEntry.Text = preferencesService.UserName;
        requirement.UsersFolderEntry.Text = preferencesService.UsersFolder;
        requirement.DemoFolderEntry.Text = preferencesService.DemoFolder;
        requirement.PythonFolderEntry.Text = preferencesService.PythonFolder;
    }

    [RelayCommand]
    public async void OpenFolderPicker(InputView input)
    {
        var ct = new CancellationToken();
        var result = await FolderPicker.Default.PickAsync(ct);
        if (result.IsSuccessful)
        {
            input.Text = result.Folder.Path;
        }
    }

    [RelayCommand]
    public void Reset()
    {
        preferencesService.Clear();
        Application.Current.MainPage.DisplayAlert("完成", $"偏好設定已重置，請重啟程式", "OK");
    }

    [RelayCommand]
    public void Save()
    {
        preferencesService.UserName = requirement.UserNameEntry.Text;
        preferencesService.UsersFolder = requirement.UsersFolderEntry.Text;
        preferencesService.DemoFolder = requirement.DemoFolderEntry.Text;
        preferencesService.PythonFolder = requirement.PythonFolderEntry.Text;
        CreateUserFolder();
        Application.Current.MainPage.DisplayAlert("完成", $"偏好設定已儲存", "OK");
    }

    private void CreateUserFolder()
    {
        var sourceFolder = Path.Combine(preferencesService.UserFolder, "Source");
        var skeletonFolder = Path.Combine(preferencesService.UserFolder, "Skeleton");
        Directory.CreateDirectory(sourceFolder);
        Directory.CreateDirectory(skeletonFolder);
    }

    [RelayCommand]
    public async void LoadVocabularyInfos()
    {
        var result = await FilePicker.Default.PickAsync();
        vocabularyService.AddVocabularyFromJsonFile(result.FullPath);
    }

    [RelayCommand]
    public async void DumpVocabularyInfos()
    {
        var ct = new CancellationToken();
        var result = await FolderPicker.Default.PickAsync(ct);
        if (result.IsSuccessful)
        {
            var path = Path.Combine(result.Folder.Path, "data.json");
            vocabularyService.Dump(path);
        }
    }

    [RelayCommand]
    public void DropVocabularyInfos()
    {
        vocabularyService.Drop();
    }
}
