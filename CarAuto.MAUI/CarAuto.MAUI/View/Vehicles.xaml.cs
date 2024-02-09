namespace CarAuto.MAUI.Pages;

public partial class Vehicles : ContentPage
{

    public Vehicles()
	{
        InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            var leave = await DisplayAlert("Exit", "Are you sure you want to exit?", "Yes", "No");

            if (leave)
            {
                Application.Current.Quit();
            }
        });

        return true;
    }
}