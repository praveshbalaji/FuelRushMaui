using System;

namespace FuelRushMaui.Models
{
    public class Vehicle
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Generation { get; set; } = "Gen 1";
        public string Category { get; set; } = "Muscle Icon";
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        
        public int Price { get; set; }
        public float TopSpeed { get; set; } // Speed multiplier
        public float Acceleration { get; set; }
        public float FuelEfficiency { get; set; } // Higher = saves fuel
        public float Handling { get; set; } // Steering responsiveness
        public float MaxRpm { get; set; } = 9000f;
        
        public string PrimaryColor { get; set; } = "#FF3366";
        public string SecondaryColor { get; set; } = "#1A1A2E";
        public string AccentColor { get; set; } = "#FFD700";
        public string UnderglowColor { get; set; } = "#00E5FF";
        
        public bool HasSpoiler { get; set; } = true;
        public bool HasRacingStripes { get; set; } = true;
        
        // --- Scenario & Achievement System ---
        public int ScenarioNumber { get; set; } = 1;
        public string AchievementTitle { get; set; } = string.Empty;
        public string AchievementDescription { get; set; } = string.Empty;
        public string AchievementType { get; set; } = "Starter"; // Starter, Level, Coins, Speed, Completion
        public float AchievementRequirement { get; set; }
        public float AchievementProgress { get; set; } // 0.0f to 1.0f
        public bool IsAchievementMet { get; set; }
        
        public bool IsUnlocked { get; set; }
        public bool IsSelected { get; set; }
    }
}
