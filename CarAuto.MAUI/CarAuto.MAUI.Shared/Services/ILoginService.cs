using CarAuto.MAUI.Shared.ViewModels;

namespace CarAuto.MAUI.Shared.Services;

public interface ILoginService
{
    Task<string> LoginAsync();

    Task RegisterAsync(UserViewModel userInfo);
}
