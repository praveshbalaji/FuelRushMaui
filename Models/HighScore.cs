using System;

namespace FuelRushMaui.Models
{
    public class HighScore
    {
        public string PlayerName { get; set; } = "Racer";
        public int Score { get; set; }
        public float DistanceKm { get; set; }
        public int CoinsCollected { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
