using System;
using Foundation;
using UIKit;

namespace FuelRushMaui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		AppDomain.CurrentDomain.UnhandledException += (s, e) =>
		{
			Console.WriteLine($"[iOS UnhandledException]: {e.ExceptionObject}");
		};

		ObjCRuntime.Runtime.MarshalManagedException += (s, e) =>
		{
			Console.WriteLine($"[iOS MarshalManagedException]: {e.Exception}");
			e.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
		};

		return base.FinishedLaunching(application, launchOptions);
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
