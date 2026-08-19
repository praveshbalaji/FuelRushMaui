using System;

namespace FuelRushMaui.Models
{
    public enum CollectibleType
    {
        FuelCanister,
        GoldCoin,
        NitroTank,
        EnergyShield,
        DoubleMultiplier
    }

    public class Collectible
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; } = 22f;
        public CollectibleType Type { get; set; }
        public bool IsActive { get; set; } = true;
        public float Rotation { get; set; }
        public float PulseScale { get; set; } = 1.0f;
        public int LaneIndex { get; set; }
    }
}
