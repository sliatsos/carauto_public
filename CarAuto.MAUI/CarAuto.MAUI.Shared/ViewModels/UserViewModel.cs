using CarAuto.MAUI.Shared.Services;
using CarAuto.Protos.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;

namespace CarAuto.MAUI.Shared.ViewModels;

public partial class UserViewModel : ObservableObject
{
    private readonly ILoginService _loginService;
    private readonly IToastManager _toastManager;

    public UserViewModel(ILoginService loginService, IToastManager toastManager)
    {
        _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        _toastManager = toastManager ?? throw new ArgumentNullException(nameof(toastManager));
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand))]
    private string firstName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand))]
    string lastName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand))]
    string userName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand))]
    string password;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand))]
    private PhoneType phoneType;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand))]
    private string phoneNumber;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterUserCommand))]
    private string email;

    [RelayCommand(CanExecute = nameof(CanRegisterUser))]
    private async Task RegisterUserAsync(UserViewModel userViewModel)
    {
        try
        {
            await _loginService.RegisterAsync(userViewModel);
            await _toastManager.ShowToastAsync("Your user has been created successfully!");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _toastManager.ShowToastAsync(ex.Message);
        }
    }

    [ObservableProperty]
    IReadOnlyCollection<string> phoneTypes = Enum.GetNames(typeof(PhoneType));

    private bool CanRegisterUser()
    {
        return !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(phoneNumber) &&
            !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password);
    }
}
