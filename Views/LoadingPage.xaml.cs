using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FuelRushMaui.Views
{
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RunTurboLoaderAnimationAsync();
        }

        private async Task RunTurboLoaderAnimationAsync()
        {
            try
            {
                double trackMaxTranslationX = 190; // Travel distance across loading track

                // Step 1: Initialize Engine
                lblStatus.Text = "⚡ PRE-WARMING V8 ENGINE...";
                await AnimateStepAsync(0.20f, 40, trackMaxTranslationX);

                // Step 2: Inject Nitro & Turbo Boost
                lblStatus.Text = "🚀 TURBO BOOST CHARGING...";
                await AnimateStepAsync(0.50f, 95, trackMaxTranslationX);

                // Step 3: Load Graphics & Assets
                lblStatus.Text = "🏁 LOADING HIGHWAY SCENARIOS...";
                await AnimateStepAsync(0.80f, 150, trackMaxTranslationX);

                // Step 4: Final Launch Acceleration
                lblStatus.Text = "🔥 LAUNCHING FUEL RUSH SIMULATOR!";
                await AnimateStepAsync(1.00f, 190, trackMaxTranslationX);

                await Task.Delay(150);

                // Smooth Navigation to AppShell
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current!.MainPage = new AppShell();
                });
            }
            catch
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current!.MainPage = new AppShell();
                });
            }
        }

        private async Task AnimateStepAsync(float targetProgress, double targetX, double maxX)
        {
            pbLoading.Progress = targetProgress;
            lblProgressPct.Text = $"{targetProgress * 100:0}%";

            // Motion animation: Move turbo car forward + engine vibration pulse
            await Task.WhenAll(
                imgTurboCar.TranslateTo(targetX, 0, 300, Easing.CubicOut),
                imgTurboCar.ScaleTo(1.08, 150, Easing.SinOut)
            );
            await imgTurboCar.ScaleTo(1.00, 150, Easing.SinIn);
            await Task.Delay(100);
        }
    }
}
