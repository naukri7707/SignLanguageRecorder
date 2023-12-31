﻿using CommunityToolkit.Maui.Core;

namespace SignLanguageRecorder.Services;

public class DialogService
{
    private static Page MainPage => Application.Current.MainPage;

    public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
    {
        return MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
    }
    
    public Task<string> DisplayActionSheet(string title, string cancel, string destruction, FlowDirection flowDirection, params string[] buttons)
    {
        return MainPage.DisplayActionSheet(title, cancel, destruction, flowDirection, buttons);
    }
    
    public Task DisplayAlert(string title, string message, string cancel)
    {
        return MainPage.DisplayAlert(title, message, cancel);
    }

    public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
    {
        return MainPage.DisplayAlert(title, message, accept, cancel);
    }
    
    public Task<bool> DisplayAlert(string title, string message, string accept, string cancel, FlowDirection flowDirection)
    {
        return MainPage.DisplayAlert(title, message,accept, cancel, flowDirection);
    }
    
    public Task DisplayAlert(string title, string message, string cancel, FlowDirection flowDirection)
    {
        return MainPage.DisplayAlert(title , message, cancel, flowDirection);
    }

    public Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
    {
        return MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
    }

    public Task Toast(
        string message,
        ToastDuration duration = ToastDuration.Short,
        double textSize = AlertDefaults.FontSize,
        CancellationToken token = default
        )
    {
        return CommunityToolkit.Maui.Alerts.Toast.Make(message, duration, textSize).Show(token);
    }

    public Task Snackbar(
        string message,
        Action action = null,
        string actionButtonText = AlertDefaults.ActionButtonText,
        TimeSpan? duration = null,
        SnackbarOptions visualOptions = null,
        IView anchor = null,
        CancellationToken token = default
        )
    {
        return CommunityToolkit.Maui.Alerts.Snackbar.Make(message, action, actionButtonText, duration, visualOptions, anchor).Show(token);
    }
}
