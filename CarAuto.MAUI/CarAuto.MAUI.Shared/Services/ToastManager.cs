using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace CarAuto.MAUI.Shared.Services;

public class ToastManager : IToastManager
{
    public async Task ShowToastAsync(string message)
    {
        var toast = Toast.Make(message, ToastDuration.Long);
        await toast.Show();
    }
}
