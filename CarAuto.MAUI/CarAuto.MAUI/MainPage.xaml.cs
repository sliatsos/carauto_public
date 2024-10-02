using CarAuto.MAUI.Shared.Configuration;
using CarAuto.MAUI.Shared.Services;
using CarAuto.MAUI.View;

namespace CarAuto.MAUI;

public partial class MainPage : ContentPage
{
    private readonly ILoginService _loginService;
    private readonly UserRegisterView _userRegisterView;

    public MainPage(ILoginService loginService, UserRegisterView userRegisterView)
    {
        _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        _userRegisterView = userRegisterView ?? throw new ArgumentNullException(nameof(userRegisterView));

        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(_userRegisterView);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            var accessToken = await _loginService.LoginAsync();

            await SecureStorage.Default.SetAsync(KeycloakConfiguration.TokenStorageName, accessToken);

            await Navigation.PushModalAsync(new Pages.Vehicles());
        }
        catch (TaskCanceledException)
        {
            //User closed browser
        }
    }
}

