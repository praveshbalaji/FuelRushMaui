using System;
using System.Collections.Generic;
using System.Linq;
using FuelRushMaui.Models;

namespace FuelRushMaui.Services
{
    public class GarageService
    {
        private readonly StorageService _storageService;

        public GarageService(StorageService storageService)
        {
            _storageService = storageService;
        }

        public List<Vehicle> GetAllVehicles()
        {
            var unlockedIds = _storageService.GetUnlockedVehicleIds();
            var selectedId = _storageService.GetSelectedVehicleId();

            int currentMaxLevel = _storageService.GetMaxLevelReached();
            float currentMaxSpeed = _storageService.GetMaxSpeedKmH();
            int totalCoins = _storageService.GetTotalCoins();
            float completionPct = _storageService.GetGameCompletionPercentage();

            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    Id = "mustang_1965",
                    Name = "1965 Mustang Fastback",
                    Generation = "Gen 1 (1965)",
                    Category = "Classic V8 Icon",
                    Description = "The legend that started it all. Wimbledon White V8 classic fastback with raw muscle rumble and timeless body lines.",
                    ImagePath = "car_mustang_1965.png",
                    Price = 0,
                    TopSpeed = 1.20f,
                    Acceleration = 1.25f,
                    FuelEfficiency = 1.00f,
                    Handling = 1.10f,
                    MaxRpm = 8000f,
                    PrimaryColor = "#F4F4F0",
                    SecondaryColor = "#0F172A",
                    AccentColor = "#0055FF",
                    UnderglowColor = "#00E5FF",
                    HasSpoiler = false,
                    HasRacingStripes = true,
                    ScenarioNumber = 1,
                    AchievementTitle = "🏆 Scenario 1: Vintage Pioneer",
                    AchievementDescription = "Default Starter Car - Ready to Race",
                    AchievementType = "Starter",
                    AchievementRequirement = 0,
                    AchievementProgress = 1.0f,
                    IsAchievementMet = true
                },
                new Vehicle
                {
                    Id = "mustang_1974",
                    Name = "1974 Mustang II Coupe",
                    Generation = "Gen 2 (1974)",
                    Category = "Retro Silver Coupe",
                    Description = "Sleek silver metallic styling with eco fuel-saving tuning. Lightweight body design built for precision distance cruising.",
                    ImagePath = "car_mustang_1974.png",
                    Price = 300,
                    TopSpeed = 1.35f,
                    Acceleration = 1.35f,
                    FuelEfficiency = 1.25f,
                    Handling = 1.18f,
                    MaxRpm = 8200f,
                    PrimaryColor = "#C0C0C0",
                    SecondaryColor = "#1E293B",
                    AccentColor = "#00E5FF",
                    UnderglowColor = "#00FFDD",
                    HasSpoiler = false,
                    HasRacingStripes = false,
                    ScenarioNumber = 2,
                    AchievementTitle = "🏆 Scenario 2: Gas Station Pioneer",
                    AchievementDescription = "Reach Gas Station #1 (Level 1 Complete)",
                    AchievementType = "Level",
                    AchievementRequirement = 1,
                    AchievementProgress = Math.Clamp(currentMaxLevel / 1.0f, 0f, 1f),
                    IsAchievementMet = (currentMaxLevel >= 1)
                },
                new Vehicle
                {
                    Id = "mustang_1990",
                    Name = "1990 Fox Body GT",
                    Generation = "Gen 3 (1990)",
                    Category = "Foxbody V8 Legend",
                    Description = "Vibrant crimson GT 5.0L. Iconic boxy 90s silhouette with massive torque output and rapid throttle response.",
                    ImagePath = "car_mustang_1990.png",
                    Price = 600,
                    TopSpeed = 1.55f,
                    Acceleration = 1.55f,
                    FuelEfficiency = 1.05f,
                    Handling = 1.28f,
                    MaxRpm = 8800f,
                    PrimaryColor = "#FF1A3C",
                    SecondaryColor = "#334155",
                    AccentColor = "#FFFFFF",
                    UnderglowColor = "#FF0055",
                    HasSpoiler = true,
                    HasRacingStripes = false,
                    ScenarioNumber = 3,
                    AchievementTitle = "🏆 Scenario 3: Coin Collector",
                    AchievementDescription = "Accumulate 100 Total Lifetime Coins",
                    AchievementType = "Coins",
                    AchievementRequirement = 100,
                    AchievementProgress = Math.Clamp(totalCoins / 100.0f, 0f, 1f),
                    IsAchievementMet = (totalCoins >= 100)
                },
                new Vehicle
                {
                    Id = "mustang_2003",
                    Name = "2003 SVT Cobra",
                    Generation = "Gen 4 (2003)",
                    Category = "Mystichrome Supercharged",
                    Description = "Iridescent Mystichrome color-shift paint with supercharged 4.6L V8 Terminator engine. High speed apex stability.",
                    ImagePath = "car_mustang_2003.png",
                    Price = 900,
                    TopSpeed = 1.75f,
                    Acceleration = 1.75f,
                    FuelEfficiency = 0.95f,
                    Handling = 1.35f,
                    MaxRpm = 9200f,
                    PrimaryColor = "#7B2CBF",
                    SecondaryColor = "#10002B",
                    AccentColor = "#00F5D4",
                    UnderglowColor = "#9D4EDD",
                    HasSpoiler = true,
                    HasRacingStripes = false,
                    ScenarioNumber = 4,
                    AchievementTitle = "🏆 Scenario 4: Velocity Master",
                    AchievementDescription = "Reach 180 KM/H Speed Milestone",
                    AchievementType = "Speed",
                    AchievementRequirement = 180,
                    AchievementProgress = Math.Clamp(currentMaxSpeed / 180.0f, 0f, 1f),
                    IsAchievementMet = (currentMaxSpeed >= 180f)
                },
                new Vehicle
                {
                    Id = "mustang_2013",
                    Name = "2013 Shelby GT500",
                    Generation = "Gen 5 (2013)",
                    Category = "Supercharged 662HP",
                    Description = "Gloss black body with dual red racing stripes. 5.8L Trinity V8 producing 662 horsepower with immense nitro acceleration.",
                    ImagePath = "car_mustang_2013.png",
                    Price = 1300,
                    TopSpeed = 1.95f,
                    Acceleration = 1.90f,
                    FuelEfficiency = 0.90f,
                    Handling = 1.32f,
                    MaxRpm = 9500f,
                    PrimaryColor = "#0A0E17",
                    SecondaryColor = "#1A202C",
                    AccentColor = "#FF0033",
                    UnderglowColor = "#FF0033",
                    HasSpoiler = true,
                    HasRacingStripes = true,
                    ScenarioNumber = 5,
                    AchievementTitle = "🏆 Scenario 5: Endurance Legend",
                    AchievementDescription = "Reach Gas Station #4 (Level 4)",
                    AchievementType = "Level",
                    AchievementRequirement = 4,
                    AchievementProgress = Math.Clamp(currentMaxLevel / 4.0f, 0f, 1f),
                    IsAchievementMet = (currentMaxLevel >= 4)
                },
                new Vehicle
                {
                    Id = "mustang_2024",
                    Name = "2024 Dark Horse",
                    Generation = "Gen 6 (2024)",
                    Category = "Hyper Spec Apex",
                    Description = "Vapor Blue Metallic track weapon. Titanium intake, active dynamic wing, and ultimate nitro boost speed for total domination.",
                    ImagePath = "car_mustang_2024.png",
                    Price = 2000,
                    TopSpeed = 2.20f,
                    Acceleration = 2.05f,
                    FuelEfficiency = 1.30f,
                    Handling = 1.50f,
                    MaxRpm = 10000f,
                    PrimaryColor = "#1E3A8A",
                    SecondaryColor = "#0F172A",
                    AccentColor = "#38BDF8",
                    UnderglowColor = "#00E5FF",
                    HasSpoiler = true,
                    HasRacingStripes = false,
                    ScenarioNumber = 6,
                    AchievementTitle = "🏆 Scenario 6: Game Completion Master",
                    AchievementDescription = "Achieve 100% Game Completion (Master Level 5)",
                    AchievementType = "Completion",
                    AchievementRequirement = 100,
                    AchievementProgress = Math.Clamp(completionPct / 100.0f, 0f, 1f),
                    IsAchievementMet = (currentMaxLevel >= 5 || completionPct >= 100f)
                }
            };

            foreach (var v in vehicles)
            {
                if (v.IsAchievementMet && !unlockedIds.Contains(v.Id))
                {
                    _storageService.UnlockVehicle(v.Id);
                    unlockedIds.Add(v.Id);
                }

                v.IsUnlocked = unlockedIds.Contains(v.Id);
                v.IsSelected = (v.Id == selectedId);
            }

            return vehicles;
        }

        public Vehicle GetSelectedVehicle()
        {
            var vehicles = GetAllVehicles();
            return vehicles.FirstOrDefault(v => v.IsSelected) ?? vehicles.First();
        }

        public bool UnlockVehicle(string vehicleId)
        {
            var vehicles = GetAllVehicles();
            var target = vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (target != null && !target.IsUnlocked)
            {
                if (target.IsAchievementMet || _storageService.DeductCoins(target.Price))
                {
                    _storageService.UnlockVehicle(vehicleId);
                    _storageService.SetSelectedVehicleId(vehicleId);
                    return true;
                }
            }
            return false;
        }

        public void SelectVehicle(string vehicleId)
        {
            var unlocked = _storageService.GetUnlockedVehicleIds();
            if (unlocked.Contains(vehicleId))
            {
                _storageService.SetSelectedVehicleId(vehicleId);
            }
        }
    }
}
