using System;
using System.Collections.Generic;
using FuelRushMaui.Models;
using FuelRushMaui.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace FuelRushMaui.Views
{
    public partial class GarageModalView : ContentView
    {
        public event Action? OnClosed;

        private GarageService? _garageService;
        private StorageService? _storageService;
        private List<Vehicle> _vehicles = new();
        private int _currentIndex = 0;

        public GarageModalView()
        {
            InitializeComponent();
        }

        public void Initialize(GarageService garageService, StorageService storageService)
        {
            _garageService = garageService;
            _storageService = storageService;
            LoadData();
        }

        public void LoadData()
        {
            if (_garageService == null || _storageService == null) return;

            lblCoins.Text = _storageService.GetTotalCoins().ToString("N0");
            _vehicles = _garageService.GetAllVehicles();

            var selected = _garageService.GetSelectedVehicle();
            _currentIndex = _vehicles.FindIndex(v => v.Id == selected.Id);
            if (_currentIndex < 0) _currentIndex = 0;

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_vehicles.Count == 0) return;

            var v = _vehicles[_currentIndex];
            
            // Image Preview & Name
            imgCarPreview.Source = v.ImagePath;
            lblGeneration.Text = v.Generation;
            lblVehicleName.Text = v.Name;
            lblVehicleDesc.Text = v.Description;

            // Lock Badge Status
            if (v.IsUnlocked)
            {
                badgeLockStatus.BackgroundColor = Color.FromArgb("#10B981");
                lblLockStatus.Text = "UNLOCKED ✓";
            }
            else if (v.IsAchievementMet)
            {
                badgeLockStatus.BackgroundColor = Color.FromArgb("#F59E0B");
                lblLockStatus.Text = "🏆 READY TO CLAIM";
            }
            else
            {
                badgeLockStatus.BackgroundColor = Color.FromArgb("#EF4444");
                lblLockStatus.Text = "🔒 LOCKED";
            }

            // Achievement / Scenario Info
            lblAchievementTitle.Text = v.AchievementTitle;
            lblAchievementDesc.Text = v.AchievementDescription;
            float progressPct = Math.Clamp(v.AchievementProgress, 0f, 1f);
            pbAchievementProgress.Progress = progressPct;
            lblAchievementPct.Text = $"{progressPct * 100:0}%";
            pbAchievementProgress.ProgressColor = progressPct >= 1.0f ? Color.FromArgb("#10B981") : Color.FromArgb("#F59E0B");

            // Specs
            lblValueSpeed.Text = $"{v.TopSpeed * 150:0} km/h";
            lblValueAccel.Text = $"{v.Acceleration:0.00}x";
            lblValueFuel.Text = $"{v.FuelEfficiency:0.00}x";
            lblValueHandling.Text = $"{v.Handling:0.00}x";

            pbSpeed.Progress = Math.Min(1.0, v.TopSpeed / 2.3);
            pbAccel.Progress = Math.Min(1.0, v.Acceleration / 2.2);
            pbFuel.Progress = Math.Min(1.0, v.FuelEfficiency / 1.5);
            pbHandling.Progress = Math.Min(1.0, v.Handling / 1.6);

            // Action Button Setup
            if (v.IsSelected)
            {
                btnAction.Text = "SELECTED VEHICLE ✓";
                btnAction.BackgroundColor = Color.FromArgb("#10B981");
                btnAction.TextColor = Color.FromArgb("#FFFFFF");
                btnAction.IsEnabled = false;
            }
            else if (v.IsUnlocked)
            {
                btnAction.Text = "SELECT VEHICLE";
                btnAction.BackgroundColor = Color.FromArgb("#00E5FF");
                btnAction.TextColor = Color.FromArgb("#0B0E14");
                btnAction.IsEnabled = true;
            }
            else if (v.IsAchievementMet)
            {
                btnAction.Text = "🏆 CLAIM FREE UNLOCK!";
                btnAction.BackgroundColor = Color.FromArgb("#F59E0B");
                btnAction.TextColor = Color.FromArgb("#0B0E14");
                btnAction.IsEnabled = true;
            }
            else
            {
                btnAction.Text = $"BUY 🪙 {v.Price} OR COMPLETE SCENARIO";
                btnAction.BackgroundColor = Color.FromArgb("#64748B");
                btnAction.TextColor = Color.FromArgb("#FFFFFF");
                btnAction.IsEnabled = (_storageService?.GetTotalCoins() >= v.Price);
            }
        }

        private void OnPrevClicked(object sender, EventArgs e)
        {
            _currentIndex = (_currentIndex - 1 + _vehicles.Count) % _vehicles.Count;
            UpdateUI();
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            _currentIndex = (_currentIndex + 1) % _vehicles.Count;
            UpdateUI();
        }

        private void OnActionClicked(object sender, EventArgs e)
        {
            if (_garageService == null || _vehicles.Count == 0) return;

            var v = _vehicles[_currentIndex];
            if (v.IsUnlocked)
            {
                _garageService.SelectVehicle(v.Id);
            }
            else if (v.IsAchievementMet || (_storageService != null && _storageService.GetTotalCoins() >= v.Price))
            {
                if (_garageService.UnlockVehicle(v.Id))
                {
                    _garageService.SelectVehicle(v.Id);
                }
            }

            LoadData();
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            OnClosed?.Invoke();
        }
    }
}
