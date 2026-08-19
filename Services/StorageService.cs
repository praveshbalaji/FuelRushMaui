using System;
using System.Collections.Generic;
using System.Text.Json;
using FuelRushMaui.Models;
using Microsoft.Maui.Storage;

namespace FuelRushMaui.Services
{
    public class StorageService
    {
        private const string KeyCoins = "fuelrush_total_coins";
        private const string KeyHighScores = "fuelrush_high_scores";
        private const string KeyUnlockedVehicles = "fuelrush_unlocked_vehicles";
        private const string KeySelectedVehicle = "fuelrush_selected_vehicle";
        private const string KeySoundEnabled = "fuelrush_sound_enabled";

        // Statistics for Achievement Scenarios
        private const string KeyMaxLevel = "fuelrush_max_level";
        private const string KeyMaxSpeed = "fuelrush_max_speed";
        private const string KeyTotalDistance = "fuelrush_total_distance";
        private const string KeyGasStationsCount = "fuelrush_gas_stations_count";

        public int GetTotalCoins()
        {
            return Preferences.Get(KeyCoins, 150); // Start with 150 bonus coins
        }

        public void AddCoins(int amount)
        {
            int current = GetTotalCoins();
            Preferences.Set(KeyCoins, Math.Max(0, current + amount));
        }

        public bool DeductCoins(int amount)
        {
            int current = GetTotalCoins();
            if (current >= amount)
            {
                Preferences.Set(KeyCoins, current - amount);
                return true;
            }
            return false;
        }

        // --- Scenario Metrics Tracking ---
        public int GetMaxLevelReached()
        {
            return Preferences.Get(KeyMaxLevel, 1);
        }

        public void UpdateMaxLevelReached(int level)
        {
            int current = GetMaxLevelReached();
            if (level > current)
            {
                Preferences.Set(KeyMaxLevel, level);
            }
        }

        public float GetMaxSpeedKmH()
        {
            return Preferences.Get(KeyMaxSpeed, 0f);
        }

        public void UpdateMaxSpeedKmH(float speed)
        {
            float current = GetMaxSpeedKmH();
            if (speed > current)
            {
                Preferences.Set(KeyMaxSpeed, speed);
            }
        }

        public float GetTotalDistanceKm()
        {
            return Preferences.Get(KeyTotalDistance, 0f);
        }

        public void AddDistanceKm(float distance)
        {
            float current = GetTotalDistanceKm();
            Preferences.Set(KeyTotalDistance, current + distance);
        }

        public int GetGasStationsReached()
        {
            return Preferences.Get(KeyGasStationsCount, 0);
        }

        public void IncrementGasStationsReached()
        {
            int current = GetGasStationsReached();
            Preferences.Set(KeyGasStationsCount, current + 1);
        }

        // Calculate Overall Game Completion Percentage (0% to 100%)
        public float GetGameCompletionPercentage()
        {
            int maxLevel = GetMaxLevelReached(); // Level 1..5
            float levelCompletion = Math.Clamp((maxLevel - 1) / 4.0f, 0f, 1f); // 0% to 100%
            return levelCompletion * 100f;
        }

        public List<HighScore> GetHighScores()
        {
            string json = Preferences.Get(KeyHighScores, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return new List<HighScore>
                {
                    new HighScore { PlayerName = "Apex Racer", Score = 12500, DistanceKm = 14.2f, CoinsCollected = 85 },
                    new HighScore { PlayerName = "Fuel King", Score = 8900, DistanceKm = 9.8f, CoinsCollected = 60 },
                    new HighScore { PlayerName = "Cyber Cruiser", Score = 5400, DistanceKm = 6.1f, CoinsCollected = 35 }
                };
            }

            try
            {
                return JsonSerializer.Deserialize<List<HighScore>>(json) ?? new List<HighScore>();
            }
            catch
            {
                return new List<HighScore>();
            }
        }

        public void SaveHighScore(HighScore score)
        {
            var scores = GetHighScores();
            scores.Add(score);
            scores.Sort((a, b) => b.Score.CompareTo(a.Score));
            if (scores.Count > 10)
            {
                scores = scores.GetRange(0, 10);
            }

            string json = JsonSerializer.Serialize(scores);
            Preferences.Set(KeyHighScores, json);

            // Update stats
            UpdateMaxSpeedKmH(score.DistanceKm * 10f); // approx metric check
            AddDistanceKm(score.DistanceKm);
            AddCoins(score.CoinsCollected);
        }

        public List<string> GetUnlockedVehicleIds()
        {
            string json = Preferences.Get(KeyUnlockedVehicles, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return new List<string> { "mustang_1965" };
            }

            try
            {
                var list = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string> { "mustang_1965" };
                if (!list.Contains("mustang_1965")) list.Add("mustang_1965");
                return list;
            }
            catch
            {
                return new List<string> { "mustang_1965" };
            }
        }

        public void UnlockVehicle(string vehicleId)
        {
            var list = GetUnlockedVehicleIds();
            if (!list.Contains(vehicleId))
            {
                list.Add(vehicleId);
                Preferences.Set(KeyUnlockedVehicles, JsonSerializer.Serialize(list));
            }
        }

        public string GetSelectedVehicleId()
        {
            return Preferences.Get(KeySelectedVehicle, "mustang_1965");
        }

        public void SetSelectedVehicleId(string vehicleId)
        {
            Preferences.Set(KeySelectedVehicle, vehicleId);
        }

        public bool IsSoundEnabled()
        {
            return Preferences.Get(KeySoundEnabled, true);
        }

        public void SetSoundEnabled(bool enabled)
        {
            Preferences.Set(KeySoundEnabled, enabled);
        }
    }
}
