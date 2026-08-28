using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FuelRushMaui.Models;
using FuelRushMaui.Services;

namespace FuelRushMaui.ViewModels
{
    public class GarageViewModel : INotifyPropertyChanged
    {
        private readonly GarageService _garageService;
        private readonly StorageService _storageService;
        private readonly GameEngine _gameEngine;

        private List<Vehicle> _vehicles = new();
        private int _currentIndex = 0;
        private Vehicle _selectedVehicle = null!;

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<Vehicle> Vehicles
        {
            get => _vehicles;
            set { _vehicles = value; OnPropertyChanged(); }
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex != value && _vehicles.Count > 0)
                {
                    _currentIndex = (value + _vehicles.Count) % _vehicles.Count;
                    CurrentVehicleView = _vehicles[_currentIndex];
                    OnPropertyChanged();
                }
            }
        }

        private Vehicle _currentVehicleView = null!;
        public Vehicle CurrentVehicleView
        {
            get => _currentVehicleView;
            set { _currentVehicleView = value; OnPropertyChanged(); }
        }

        public Vehicle SelectedVehicle
        {
            get => _selectedVehicle;
            set
            {
                if (_selectedVehicle != value)
                {
                    _selectedVehicle = value;
                    OnPropertyChanged();
                    // Bind & sync state directly to GameEngine
                    _gameEngine.SetSelectedVehicle(_selectedVehicle);
                }
            }
        }

        public GarageViewModel(GarageService garageService, StorageService storageService, GameEngine gameEngine)
        {
            _garageService = garageService;
            _storageService = storageService;
            _gameEngine = gameEngine;
            LoadVehicles();
        }

        public void LoadVehicles()
        {
            Vehicles = _garageService.GetAllVehicles();
            var selected = _garageService.GetSelectedVehicle();
            _selectedVehicle = selected;
            OnPropertyChanged(nameof(SelectedVehicle));

            _gameEngine?.SetSelectedVehicle(_selectedVehicle);

            _currentIndex = Vehicles.FindIndex(v => v.Id == _selectedVehicle.Id);
            if (_currentIndex < 0) _currentIndex = 0;
            if (Vehicles.Count > 0)
            {
                CurrentVehicleView = Vehicles[_currentIndex];
            }
        }

        public void SelectCurrentVehicle()
        {
            if (CurrentVehicleView == null) return;

            if (CurrentVehicleView.IsUnlocked)
            {
                _garageService.SelectVehicle(CurrentVehicleView.Id);
                SelectedVehicle = CurrentVehicleView;
            }
            else if (CurrentVehicleView.IsAchievementMet || _storageService.GetTotalCoins() >= CurrentVehicleView.Price)
            {
                if (_garageService.UnlockVehicle(CurrentVehicleView.Id))
                {
                    _garageService.SelectVehicle(CurrentVehicleView.Id);
                    SelectedVehicle = CurrentVehicleView;
                }
            }

            // Explicitly sync SelectedVehicle to GameEngine
            _gameEngine?.SetSelectedVehicle(SelectedVehicle);

            // Update IsSelected flag across all vehicles
            foreach (var v in Vehicles)
            {
                v.IsSelected = (v.Id == SelectedVehicle.Id);
            }

            CurrentVehicleView = Vehicles[_currentIndex];
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
