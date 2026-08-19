using System;
using FuelRushMaui.Services;
using Microsoft.Maui.Controls;

namespace FuelRushMaui.Views
{
    public partial class HighScoresModalView : ContentView
    {
        public event Action? OnClosed;

        public HighScoresModalView()
        {
            InitializeComponent();
        }

        public void LoadData(StorageService storageService)
        {
            cvScores.ItemsSource = storageService.GetHighScores();
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            OnClosed?.Invoke();
        }
    }
}
