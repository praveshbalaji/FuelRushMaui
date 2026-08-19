using System;

namespace FuelRushMaui.Models
{
    public enum ParticleType
    {
        Spark,
        Explosion,
        NitroFlame,
        Smoke,
        CoinGlow,
        SpeedLine
    }

    public class Particle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VX { get; set; }
        public float VY { get; set; }
        public float Life { get; set; } // 1.0 down to 0.0
        public float MaxLife { get; set; } = 1.0f;
        public float Size { get; set; }
        public string ColorHex { get; set; } = "#FFCC00";
        public ParticleType Type { get; set; }
    }
}
