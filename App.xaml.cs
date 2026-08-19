using Microsoft.Extensions.DependencyInjection;

namespace FuelRushMaui;

public partial class App : Application
{
	public static event Action? OnAppSleeping;
	public static event Action? OnAppResuming;

	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new Views.LoadingPage());
		window.Deactivated += (s, e) => OnAppSleeping?.Invoke();
		window.Activated += (s, e) => OnAppResuming?.Invoke();
		return window;
	}

	protected override void OnSleep()
	{
		base.OnSleep();
		OnAppSleeping?.Invoke();
	}

	protected override void OnResume()
	{
		base.OnResume();
		OnAppResuming?.Invoke();
	}
}