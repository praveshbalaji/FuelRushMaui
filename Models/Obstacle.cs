using System;

namespace FuelRushMaui.Models
{
    public enum ObstacleType
    {
        SedanCar,
        SportsCar,
        DeliveryTruck,
        PoliceCar,
        OilSlick,
        RoadBarrier
    }

    public class Obstacle
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; } = 48f;
        public float Height { get; set; } = 90f;
        public float SpeedY { get; set; } // Relative down speed
        public ObstacleType Type { get; set; }
        public int LaneIndex { get; set; }
        public string ColorHex { get; set; } = "#3399FF";
        public bool IsActive { get; set; } = true;
        public bool HasPassedPlayer { get; set; } = false;
    }
}
