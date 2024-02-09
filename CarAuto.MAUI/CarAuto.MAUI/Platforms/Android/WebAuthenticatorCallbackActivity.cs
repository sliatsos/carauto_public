using Android.App;
using Android.Content.PM;
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent = Android.Content.Intent;

namespace CarAuto.MAUI.Platforms.Android;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
              Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
              DataScheme = CALLBACK_SCHEME)]
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
{
    const string CALLBACK_SCHEME = "carauto";

    protected override void OnResume()
    {
        base.OnResume();
        Platform.OnResume();
    }
}
