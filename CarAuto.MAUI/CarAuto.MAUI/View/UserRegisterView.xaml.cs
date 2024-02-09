using CarAuto.MAUI.Shared.ViewModels;

namespace CarAuto.MAUI.View;

public partial class UserRegisterView : ContentPage
{
	public UserRegisterView(UserViewModel userViewModel)
	{
		InitializeComponent();
		BindingContext = userViewModel;
	}
}