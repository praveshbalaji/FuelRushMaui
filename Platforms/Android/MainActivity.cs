using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace FuelRushMaui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.SensorLandscape, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        EnableImmersiveFullScreen();
    }

    protected override void OnResume()
    {
        base.OnResume();
        EnableImmersiveFullScreen();
    }

    private void EnableImmersiveFullScreen()
    {
        if (Window == null) return;

        WindowCompat.SetDecorFitsSystemWindows(Window, false);
        var windowInsetsController = WindowCompat.GetInsetsController(Window, Window.DecorView);
        if (windowInsetsController != null)
        {
            windowInsetsController.Hide(WindowInsetsCompat.Type.StatusBars() | WindowInsetsCompat.Type.NavigationBars());
            windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
        }
    }
}
