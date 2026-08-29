using System;
using FuelRushMaui.Models;
using FuelRushMaui.Services;
using Microsoft.Maui.Graphics;

namespace FuelRushMaui.Renderers
{
    public class GameCanvasDrawable : IDrawable
    {
        private readonly GameEngine _engine;
        private float _animTimer = 0f;

        public GameCanvasDrawable(GameEngine engine)
        {
            _engine = engine;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            _animTimer += 0.05f;

            _engine.CanvasWidth = dirtyRect.Width;
            _engine.CanvasHeight = dirtyRect.Height;

            canvas.Antialias = true;

            // Apply Camera Shake on Nitro / Explosions
            canvas.SaveState();
            if (_engine.CameraShakeX != 0f || _engine.CameraShakeY != 0f)
            {
                canvas.Translate(_engine.CameraShakeX, _engine.CameraShakeY);
            }

            // 1. Draw Widescreen Environment & 5-Lane Highway
            DrawLandscapeHighway(canvas, dirtyRect);

            // 2. Draw Collectibles
            DrawCollectibles(canvas);

            // 3. Draw Obstacles & AI Traffic
            DrawObstacles(canvas);

            // 4. Draw Player NFS Car
            DrawPlayerNFSCar(canvas);

            // 5. Draw Particles (Sparks, Smoke, Nitro flames)
            DrawParticles(canvas);

            // 6. Draw Analog Tachometer & Minimap Radar
            if (_engine.State == GameState.Playing)
            {
                DrawAnalogTachometer(canvas, dirtyRect);
                DrawMinimapRadar(canvas, dirtyRect);
            }

            canvas.RestoreState();
        }

        private void DrawLandscapeHighway(ICanvas canvas, RectF bounds)
        {
            // Background Night Sky Gradient
            canvas.FillColor = Color.FromArgb("#070A10");
            canvas.FillRectangle(bounds);

            float rLeft = _engine.RoadLeft;
            float rWidth = _engine.RoadWidth;
            float rRight = _engine.RoadRight;
            float laneW = _engine.LaneWidth;
            int laneCount = _engine.LaneCount;

            // Off-road Scenery Terrain
            canvas.FillColor = Color.FromArgb("#0D111A");
            canvas.FillRectangle(0, 0, rLeft, bounds.Height);
            canvas.FillRectangle(rRight, 0, bounds.Width - rRight, bounds.Height);

            // Street Light Poles along the Highway Borders
            float poleSpacing = 120f;
            float poleScroll = _engine.RoadScrollY % poleSpacing;
            canvas.StrokeColor = Color.FromRgba(0, 229, 255, 120);
            canvas.StrokeSize = 2;
            for (float y = -poleSpacing + poleScroll; y < bounds.Height + poleSpacing; y += poleSpacing)
            {
                canvas.DrawLine(rLeft - 18, y, rLeft - 8, y);
                canvas.FillColor = Color.FromArgb("#00E5FF");
                canvas.FillCircle(rLeft - 18, y, 4);

                canvas.DrawLine(rRight + 18, y, rRight + 8, y);
                canvas.FillColor = Color.FromArgb("#00E5FF");
                canvas.FillCircle(rRight + 18, y, 4);
            }

            // Main Asphalt Surface
            canvas.FillColor = Color.FromArgb("#161C27");
            canvas.FillRectangle(rLeft, 0, rWidth, bounds.Height);

            // Neon Glowing Curb Borders
            canvas.StrokeColor = Color.FromArgb("#00E5FF");
            canvas.StrokeSize = 4f;
            canvas.DrawLine(rLeft, 0, rLeft, bounds.Height);
            canvas.DrawLine(rRight, 0, rRight, bounds.Height);

            // Checkered Curb Blocks
            float curbH = 26f;
            float curbScroll = _engine.RoadScrollY % (curbH * 2);
            for (float y = -curbH * 2 + curbScroll; y < bounds.Height + curbH; y += curbH * 2)
            {
                canvas.FillColor = Color.FromArgb("#FF0055");
                canvas.FillRectangle(rLeft - 8, y, 8, curbH);
                canvas.FillRectangle(rRight, y, 8, curbH);

                canvas.FillColor = Color.FromArgb("#FFFFFF");
                canvas.FillRectangle(rLeft - 8, y + curbH, 8, curbH);
                canvas.FillRectangle(rRight, y + curbH, 8, curbH);
            }

            // 5-Lane Dashed Divider Lines
            canvas.StrokeColor = Color.FromArgb("#3B4559");
            canvas.StrokeSize = 3f;

            float dashLength = 36f;
            float gapLength = 30f;
            float totalPattern = dashLength + gapLength;
            float scrollOffset = _engine.RoadScrollY % totalPattern;

            for (int i = 1; i < laneCount; i++)
            {
                float lx = rLeft + i * laneW;
                for (float y = -totalPattern + scrollOffset; y < bounds.Height + totalPattern; y += totalPattern)
                {
                    canvas.DrawLine(lx, y, lx, y + dashLength);
                }
            }

            // Speed Blur Streak Lines during Nitro Boost
            if (_engine.IsNitroActive)
            {
                canvas.StrokeColor = Color.FromRgba(0, 229, 255, 160);
                canvas.StrokeSize = 2f;
                Random rnd = new Random(5678);
                for (int i = 0; i < 20; i++)
                {
                    float sx = (float)rnd.NextDouble() * bounds.Width;
                    float sy = (float)rnd.NextDouble() * bounds.Height;
                    canvas.DrawLine(sx, sy, sx, sy + 80f);
                }
            }
        }

        private void DrawCollectibles(ICanvas canvas)
        {
            foreach (var item in _engine.Collectibles)
            {
                if (!item.IsActive) continue;

                canvas.SaveState();
                canvas.Translate(item.X, item.Y);

                switch (item.Type)
                {
                    case CollectibleType.FuelCanister:
                        // Red Jerrycan
                        canvas.FillColor = Color.FromArgb("#FF1A40");
                        canvas.FillRoundedRectangle(-18, -22, 36, 44, 8);
                        canvas.FillColor = Color.FromArgb("#FFD700");
                        canvas.FillRectangle(-8, -28, 16, 6);
                        canvas.FontColor = Color.FromArgb("#FFFFFF");
                        canvas.FontSize = 12;
                        canvas.DrawString("GAS", -18, -10, 36, 20, HorizontalAlignment.Center, VerticalAlignment.Center);
                        break;

                    case CollectibleType.GoldCoin:
                        float scaleX = (float)Math.Abs(Math.Cos(item.Rotation * Math.PI / 180.0));
                        scaleX = Math.Max(0.2f, scaleX);
                        canvas.Scale(scaleX, 1.0f);

                        canvas.FillColor = Color.FromArgb("#FFD700");
                        canvas.FillCircle(0, 0, 20);
                        canvas.StrokeColor = Color.FromArgb("#FFA500");
                        canvas.StrokeSize = 3;
                        canvas.DrawCircle(0, 0, 20);
                        canvas.FillColor = Color.FromArgb("#FFF5A6");
                        canvas.FillCircle(0, 0, 11);
                        break;

                    case CollectibleType.NitroTank:
                        canvas.FillColor = Color.FromArgb("#00E5FF");
                        canvas.FillRoundedRectangle(-16, -24, 32, 48, 8);
                        canvas.FillColor = Color.FromArgb("#E0E0E0");
                        canvas.FillRectangle(-6, -30, 12, 8);
                        canvas.StrokeColor = Color.FromArgb("#FFFFFF");
                        canvas.StrokeSize = 3;
                        canvas.DrawLine(-4, -12, 4, -4);
                        canvas.DrawLine(4, -4, -2, 2);
                        canvas.DrawLine(-2, 2, 5, 12);
                        break;

                    case CollectibleType.EnergyShield:
                        canvas.FillColor = Color.FromRgba(0, 255, 136, 120);
                        canvas.FillCircle(0, 0, 24);
                        canvas.StrokeColor = Color.FromArgb("#00FF88");
                        canvas.StrokeSize = 3;
                        canvas.DrawCircle(0, 0, 24);
                        break;

                    case CollectibleType.DoubleMultiplier:
                        canvas.FillColor = Color.FromArgb("#B000FF");
                        canvas.FillCircle(0, 0, 22);
                        canvas.FontColor = Color.FromArgb("#FFFFFF");
                        canvas.FontSize = 14;
                        canvas.DrawString("2X", -16, -12, 32, 24, HorizontalAlignment.Center, VerticalAlignment.Center);
                        break;
                }

                canvas.RestoreState();
            }
        }

        private void DrawObstacles(ICanvas canvas)
        {
            foreach (var obs in _engine.Obstacles)
            {
                if (!obs.IsActive) continue;

                canvas.SaveState();
                canvas.Translate(obs.X, obs.Y);

                if (obs.Type == ObstacleType.OilSlick)
                {
                    // Oil Slick Reflective Hazard Puddle
                    canvas.FillColor = Color.FromRgba(18, 20, 29, 230);
                    canvas.FillEllipse(-obs.Width / 2f, -obs.Height / 2f, obs.Width, obs.Height);
                    canvas.StrokeColor = Color.FromRgba(0, 229, 255, 180);
                    canvas.StrokeSize = 2;
                    canvas.DrawEllipse(-obs.Width / 2f + 4, -obs.Height / 2f + 4, obs.Width - 8, obs.Height - 8);
                    
                    // Rainbow sheen reflection
                    canvas.FillColor = Color.FromRgba(255, 0, 128, 80);
                    canvas.FillCircle(-obs.Width * 0.15f, -obs.Height * 0.1f, obs.Width * 0.25f);
                }
                else if (obs.Type == ObstacleType.RoadBarrier)
                {
                    // Construction Hazard Barrier
                    canvas.FillColor = Color.FromArgb("#FF5500");
                    canvas.FillRoundedRectangle(-obs.Width / 2f, -obs.Height / 2f, obs.Width, obs.Height, 4);
                    canvas.FillColor = Color.FromArgb("#FFFFFF");
                    canvas.FillRectangle(-16, -obs.Height / 2f, 10, obs.Height);
                    canvas.FillRectangle(6, -obs.Height / 2f, 10, obs.Height);
                    
                    // Warning Flasher
                    bool blink = Math.Sin(_animTimer * 10) > 0;
                    canvas.FillColor = blink ? Color.FromArgb("#FFD700") : Color.FromArgb("#AA7700");
                    canvas.FillCircle(0, -obs.Height / 2f - 4, 5);
                }
                else if (obs.Type == ObstacleType.DeliveryTruck)
                {
                    float w = obs.Width;
                    float h = obs.Height;

                    // Shadow
                    canvas.FillColor = Color.FromRgba(0, 0, 0, 120);
                    canvas.FillRoundedRectangle(-w / 2f + 5, -h / 2f + 8, w, h, 8);

                    // Heavy Cargo Box Body
                    canvas.FillColor = Color.FromArgb("#D97706");
                    canvas.FillRoundedRectangle(-w / 2f, -h / 2f, w, h * 0.72f, 6);
                    canvas.FillColor = Color.FromArgb("#B45309");
                    canvas.FillRectangle(-w / 2f + 3, -h / 2f + 3, w - 6, 8);

                    // Truck Cabin
                    canvas.FillColor = Color.FromArgb("#451A03");
                    canvas.FillRoundedRectangle(-w / 2f + 2, h * 0.22f, w - 4, h * 0.28f, 6);

                    // Windshield (facing down)
                    canvas.FillColor = Color.FromArgb("#38BDF8");
                    canvas.FillRoundedRectangle(-w / 2f + 5, h * 0.35f, w - 10, h * 0.12f, 3);

                    // Headlights (facing down)
                    canvas.FillColor = Color.FromArgb("#FFFBEB");
                    canvas.FillRectangle(-w / 2f + 4, h / 2f - 4, 10, 4);
                    canvas.FillRectangle(w / 2f - 14, h / 2f - 4, 10, 4);

                    // Brake Lights (top)
                    canvas.FillColor = Color.FromArgb("#DC2626");
                    canvas.FillRectangle(-w / 2f + 4, -h / 2f, 10, 4);
                    canvas.FillRectangle(w / 2f - 14, -h / 2f, 10, 4);
                }
                else
                {
                    // Dynamic Sedan & Police Highway Traffic Models
                    float w = obs.Width;
                    float h = obs.Height;

                    // Drop Shadow
                    canvas.FillColor = Color.FromRgba(0, 0, 0, 130);
                    canvas.FillRoundedRectangle(-w / 2f + 4, -h / 2f + 8, w, h, 10);

                    // Main Chassis Body
                    canvas.FillColor = Color.FromArgb(obs.ColorHex);
                    canvas.FillRoundedRectangle(-w / 2f, -h / 2f, w, h, 10);

                    // Windshield & Roof Glass
                    canvas.FillColor = Color.FromArgb("#0F172A");
                    canvas.FillRoundedRectangle(-w / 2f + 5, -h * 0.25f, w - 10, h * 0.50f, 6);

                    canvas.FillColor = Color.FromRgba(56, 189, 248, 160);
                    canvas.FillRoundedRectangle(-w / 2f + 7, h * 0.10f, w - 14, h * 0.12f, 3);

                    // Wheels
                    canvas.FillColor = Color.FromArgb("#111111");
                    canvas.FillRoundedRectangle(-w / 2f - 3, -h * 0.35f, 4, 16, 2);
                    canvas.FillRoundedRectangle(w / 2f - 1, -h * 0.35f, 4, 16, 2);
                    canvas.FillRoundedRectangle(-w / 2f - 3, h * 0.18f, 4, 16, 2);
                    canvas.FillRoundedRectangle(w / 2f - 1, h * 0.18f, 4, 16, 2);

                    // Headlights (facing traffic direction down)
                    canvas.FillColor = Color.FromArgb("#FEF08A");
                    canvas.FillRectangle(-w / 2f + 4, h / 2f - 4, 10, 4);
                    canvas.FillRectangle(w / 2f - 14, h / 2f - 4, 10, 4);

                    // Brake Lights (top)
                    canvas.FillColor = Color.FromArgb("#EF4444");
                    canvas.FillRectangle(-w / 2f + 4, -h / 2f, 10, 4);
                    canvas.FillRectangle(w / 2f - 14, -h / 2f, 10, 4);

                    if (obs.Type == ObstacleType.PoliceCar)
                    {
                        // Highway Patrol Dual Police Strobe Lights
                        bool flash = Math.Sin(_animTimer * 18) > 0;
                        canvas.FillColor = flash ? Color.FromArgb("#EF4444") : Color.FromArgb("#2563EB");
                        canvas.FillRectangle(-12, -4, 10, 8);
                        canvas.FillColor = !flash ? Color.FromArgb("#EF4444") : Color.FromArgb("#2563EB");
                        canvas.FillRectangle(2, -4, 10, 8);
                    }
                }

                canvas.RestoreState();
            }
        }

        private void DrawPlayerNFSCar(ICanvas canvas)
        {
            float px = _engine.PlayerX;
            float py = _engine.PlayerY;
            float w = _engine.PlayerWidth;
            float h = _engine.PlayerHeight;
            var vehicle = _engine.CurrentVehicle ?? new Vehicle();

            canvas.SaveState();
            canvas.Translate(px, py);

            // Rotate car body slightly when steering
            canvas.Rotate(_engine.SteeringAngle * 0.35f);

            // 1. Dynamic Neon Underglow LED Lighting (Uses vehicle accent/underglow color)
            Color underglowCol = Color.FromArgb(vehicle.UnderglowColor ?? "#00E5FF");
            canvas.FillColor = Color.FromRgba(underglowCol.Red, underglowCol.Green, underglowCol.Blue, 0.55f);
            canvas.FillEllipse(-w * 0.95f, -h * 0.65f, w * 1.90f, h * 1.30f);

            // 2. High-Beam Forward Headlight Cones
            canvas.FillColor = Color.FromRgba(255, 255, 220, 55);
            PathF lightBeam = new PathF();
            lightBeam.MoveTo(-w * 0.38f, -h * 0.4f);
            lightBeam.LineTo(-w * 1.70f, -h * 3.5f);
            lightBeam.LineTo(w * 1.70f, -h * 3.5f);
            lightBeam.LineTo(w * 0.38f, -h * 0.4f);
            lightBeam.Close();
            canvas.FillPath(lightBeam);

            // 3. Chassis Drop Shadow
            canvas.FillColor = Color.FromRgba(0, 0, 0, 150);
            canvas.FillRoundedRectangle(-w / 2f + 4, -h / 2f + 8, w, h, 14);

            // 4. Vehicle Colors & Distinct Body Shape by Model Id
            Color priColor = Color.FromArgb(vehicle.PrimaryColor);
            Color secColor = Color.FromArgb(vehicle.SecondaryColor);
            Color accColor = Color.FromArgb(vehicle.AccentColor);

            string carId = vehicle.Id ?? "mustang_1965";

            // Chassis Main Body Base
            canvas.FillColor = priColor;
            
            if (carId == "mustang_1990")
            {
                // 1990 Fox Body: Blocky 90s aerodynamic square GT silhouette
                canvas.FillRoundedRectangle(-w / 2f, -h / 2f, w, h, 6);
            }
            else if (carId == "mustang_1974")
            {
                // 1974 Coupe: Sleek 70s fastback contour
                canvas.FillRoundedRectangle(-w / 2f, -h / 2f, w, h, 18);
            }
            else if (carId == "mustang_2024")
            {
                // 2024 Dark Horse: Wide-body track spec
                canvas.FillRoundedRectangle(-w / 2f - 2, -h / 2f, w + 4, h, 12);
            }
            else
            {
                // Classic Fastback & Shelby GT
                canvas.FillRoundedRectangle(-w / 2f, -h / 2f, w, h, 14);
            }

            // Front Bumper & Splitter
            canvas.FillColor = accColor;
            canvas.FillRoundedRectangle(-w / 2f + 2, -h / 2f - 3, w - 4, 7, 3);

            // Metallic Body Highlight Shading
            canvas.FillColor = Color.FromRgba(255, 255, 255, 45);
            canvas.FillRoundedRectangle(-w / 2f + 3, -h / 2f + 4, w - 6, h * 0.35f, 10);

            // Model Specific Striping & Hood Accents
            if (carId == "mustang_1965")
            {
                // Classic 1965 Dual Wimbledon Blue Stripes
                canvas.FillColor = accColor;
                canvas.FillRectangle(-7, -h / 2f, 5, h * 0.92f);
                canvas.FillRectangle(2, -h / 2f, 5, h * 0.92f);

                // Chrome Front Grille & Round Mustang Badge
                canvas.FillColor = Color.FromArgb("#E2E8F0");
                canvas.FillCircle(0, -h / 2f + 6, 5);
            }
            else if (carId == "mustang_1974")
            {
                // Silver Metallic Center Stripe + Hood Scoop Bulge
                canvas.FillColor = Color.FromArgb("#00E5FF");
                canvas.FillRectangle(-3, -h / 2f, 6, h * 0.88f);
                canvas.FillColor = secColor;
                canvas.FillRoundedRectangle(-6, -h * 0.28f, 12, 16, 4);
            }
            else if (carId == "mustang_1990")
            {
                // Foxbody Black Rear Window Louvers & White Accent Strip
                canvas.FillColor = Color.FromArgb("#000000");
                canvas.FillRectangle(-w / 2f + 4, h * 0.05f, w - 8, 3);
                canvas.FillRectangle(-w / 2f + 4, h * 0.12f, w - 8, 3);
                canvas.FillRectangle(-w / 2f + 4, h * 0.19f, w - 8, 3);

                canvas.FillColor = Color.FromArgb("#FFFFFF");
                canvas.FillRectangle(-w / 2f, h * 0.32f, w, 4);
            }
            else if (carId == "mustang_2003")
            {
                // Mystichrome SVT Twin Supercharged Hood Vents
                canvas.FillColor = Color.FromArgb("#10002B");
                canvas.FillRoundedRectangle(-w * 0.32f, -h * 0.30f, 8, 14, 2);
                canvas.FillRoundedRectangle(w * 0.32f - 8, -h * 0.30f, 8, 14, 2);
            }
            else if (carId == "mustang_2013")
            {
                // 2013 Shelby Gloss Black Dual Crimson Red Stripes
                canvas.FillColor = Color.FromArgb("#FF0033");
                canvas.FillRectangle(-8, -h / 2f, 6, h * 0.94f);
                canvas.FillRectangle(2, -h / 2f, 6, h * 0.94f);
            }
            else if (carId == "mustang_2024")
            {
                // 2024 Dark Horse Cyan Air Intakes & Hood Extractor
                canvas.FillColor = Color.FromArgb("#38BDF8");
                canvas.FillRoundedRectangle(-10, -h * 0.34f, 20, 14, 4);
                canvas.FillColor = Color.FromArgb("#0F172A");
                canvas.FillRectangle(-6, -h * 0.30f, 12, 6);
            }
            else if (vehicle.HasRacingStripes)
            {
                canvas.FillColor = accColor;
                canvas.FillRectangle(-7, -h / 2f, 5, h * 0.92f);
                canvas.FillRectangle(2, -h / 2f, 5, h * 0.92f);
            }

            // Windshield & Side Windows
            canvas.FillColor = secColor;
            canvas.FillRoundedRectangle(-w / 2f + 6, -h * 0.28f, w - 12, h * 0.52f, 8);

            canvas.FillColor = Color.FromRgba(56, 189, 248, 180);
            canvas.FillRoundedRectangle(-w / 2f + 8, -h * 0.22f, w - 16, h * 0.20f, 4);

            // Dynamic Steered Front Wheels
            canvas.FillColor = Color.FromArgb("#0F172A");
            
            canvas.SaveState();
            // Front Left Steered Wheel
            canvas.Translate(-w / 2f - 4, -h * 0.32f);
            canvas.Rotate(_engine.SteeringAngle * 0.5f);
            canvas.FillRoundedRectangle(-4, -11, 7, 22, 3);
            canvas.StrokeColor = accColor;
            canvas.StrokeSize = 1.5f;
            canvas.DrawRoundedRectangle(-4, -11, 7, 22, 3);
            canvas.RestoreState();

            canvas.SaveState();
            // Front Right Steered Wheel
            canvas.Translate(w / 2f + 4, -h * 0.32f);
            canvas.Rotate(_engine.SteeringAngle * 0.5f);
            canvas.FillRoundedRectangle(-3, -11, 7, 22, 3);
            canvas.StrokeColor = accColor;
            canvas.StrokeSize = 1.5f;
            canvas.DrawRoundedRectangle(-3, -11, 7, 22, 3);
            canvas.RestoreState();

            // Heavy Rear Performance Wheels
            canvas.FillRoundedRectangle(-w / 2f - 7, h * 0.18f, 7, 22, 3);
            canvas.FillRoundedRectangle(w / 2f + 0, h * 0.18f, 7, 22, 3);

            // High Performance GT Rear Spoiler Wing
            if (vehicle.HasSpoiler || carId == "mustang_2024" || carId == "mustang_2013" || carId == "mustang_2003" || carId == "mustang_1990")
            {
                canvas.FillColor = Color.FromArgb("#0A0E17");
                canvas.FillRoundedRectangle(-w / 2f - 6, h / 2f - 8, w + 12, 8, 3);
                canvas.StrokeColor = accColor;
                canvas.StrokeSize = 2;
                canvas.DrawLine(-w / 2f - 6, h / 2f - 4, w / 2f + 6, h / 2f - 4);
            }

            // Tail Brake Lights Bar
            canvas.FillColor = Color.FromArgb("#FF0033");
            canvas.FillRectangle(-w / 2f + 4, h / 2f - 4, 12, 4);
            canvas.FillRectangle(w / 2f - 16, h / 2f - 4, 12, 4);

            // Nitro Exhaust Flame Cones
            if (_engine.IsNitroActive)
            {
                canvas.FillColor = Color.FromArgb("#00E5FF");
                canvas.FillCircle(-10, h / 2f + 14, 11);
                canvas.FillCircle(10, h / 2f + 14, 11);

                canvas.FillColor = Color.FromArgb("#FFFFFF");
                canvas.FillCircle(-10, h / 2f + 8, 5);
                canvas.FillCircle(10, h / 2f + 8, 5);
            }

            // Energy Shield Forcefield Halo
            if (_engine.IsShieldActive)
            {
                canvas.FillColor = Color.FromRgba(0, 255, 136, 75);
                canvas.FillCircle(0, 0, h * 0.78f);
                canvas.StrokeColor = Color.FromArgb("#00FF88");
                canvas.StrokeSize = 3.5f;
                canvas.DrawCircle(0, 0, h * 0.78f);
            }

            canvas.RestoreState();
        }

        private void DrawParticles(ICanvas canvas)
        {
            foreach (var p in _engine.Particles)
            {
                canvas.SaveState();
                canvas.Translate(p.X, p.Y);

                Color col = Color.FromArgb(p.ColorHex);
                canvas.FillColor = Color.FromRgba(col.Red, col.Green, col.Blue, p.Life);
                canvas.FillCircle(0, 0, p.Size * p.Life);

                canvas.RestoreState();
            }
        }

        private void DrawAnalogTachometer(ICanvas canvas, RectF bounds)
        {
            // Positioned at top dashboard HUD (no bottom UI overlap)
            float cx = bounds.Width / 2f;
            float cy = 42f;
            float radius = 34f;

            canvas.SaveState();
            canvas.Translate(cx, cy);

            // Dark Glass Backplate
            canvas.FillColor = Color.FromRgba(12, 16, 26, 220);
            canvas.FillCircle(0, 0, radius);
            canvas.StrokeColor = Color.FromArgb("#00E5FF");
            canvas.StrokeSize = 2f;
            canvas.DrawCircle(0, 0, radius);

            // RPM Ticks
            float maxRpm = _engine.CurrentVehicle.MaxRpm;
            float currentRpm = _engine.CurrentRpm;

            for (int i = 0; i <= 8; i++)
            {
                float angleDeg = 135f + (i / 8f) * 270f;
                float angleRad = (float)(angleDeg * Math.PI / 180.0);
                float x1 = (float)Math.Cos(angleRad) * (radius - 9f);
                float y1 = (float)Math.Sin(angleRad) * (radius - 9f);
                float x2 = (float)Math.Cos(angleRad) * (radius - 3f);
                float y2 = (float)Math.Sin(angleRad) * (radius - 3f);

                canvas.StrokeColor = (i >= 6) ? Color.FromArgb("#FF0033") : Color.FromArgb("#FFFFFF");
                canvas.StrokeSize = (i >= 6) ? 2.5f : 1.5f;
                canvas.DrawLine(x1, y1, x2, y2);
            }

            // Sweeping Red Needle
            float rpmRatio = Math.Clamp(currentRpm / maxRpm, 0f, 1f);
            float needleAngleDeg = 135f + rpmRatio * 270f;
            float needleAngleRad = (float)(needleAngleDeg * Math.PI / 180.0);

            float nx = (float)Math.Cos(needleAngleRad) * (radius - 7f);
            float ny = (float)Math.Sin(needleAngleRad) * (radius - 7f);

            canvas.StrokeColor = Color.FromArgb("#FF0055");
            canvas.StrokeSize = 2.5f;
            canvas.DrawLine(0, 0, nx, ny);

            canvas.FillColor = Color.FromArgb("#FFFFFF");
            canvas.FillCircle(0, 0, 4);

            // Digital Gear Indicator
            canvas.FontColor = Color.FromArgb("#FFD700");
            canvas.FontSize = 11;
            canvas.DrawString($"G{_engine.CurrentGear}", -12, 8, 24, 14, HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.RestoreState();
        }

        private void DrawSimulatorSteeringWheel(ICanvas canvas, RectF bounds)
        {
            // Positioned at bottom-left HUD (Truck/Car Simulator style)
            float cx = 110f;
            float cy = bounds.Height - 110f;
            float radius = 72f;

            canvas.SaveState();
            canvas.Translate(cx, cy);

            // Ambient Glow Ring
            canvas.FillColor = Color.FromRgba(0, 229, 255, 25);
            canvas.FillCircle(0, 0, radius + 10);

            // Apply Dynamic Real-Time Steering Rotation
            canvas.Rotate(_engine.SteeringAngle);

            // Outer Leather Rim
            canvas.StrokeColor = Color.FromArgb("#1E293B");
            canvas.StrokeSize = 18f;
            canvas.DrawCircle(0, 0, radius);

            // Outer/Inner Metal Rings
            canvas.StrokeColor = Color.FromArgb("#00E5FF");
            canvas.StrokeSize = 2f;
            canvas.DrawCircle(0, 0, radius + 9);
            canvas.DrawCircle(0, 0, radius - 9);

            // Red Racing Top Center Marker (12 o'clock line)
            canvas.FillColor = Color.FromArgb("#FF0055");
            canvas.FillRectangle(-4, -radius - 9, 8, 18);

            // 3 Heavy Silver Spokes
            canvas.StrokeColor = Color.FromArgb("#475569");
            canvas.StrokeSize = 12f;
            canvas.DrawLine(0, 0, 0, -radius + 8);
            canvas.DrawLine(0, 0, -radius * 0.866f, radius * 0.5f);
            canvas.DrawLine(0, 0, radius * 0.866f, radius * 0.5f);

            // Center Hub Cap
            canvas.FillColor = Color.FromArgb("#0A0E17");
            canvas.FillCircle(0, 0, 28);
            canvas.StrokeColor = Color.FromArgb("#FFD700");
            canvas.StrokeSize = 3f;
            canvas.DrawCircle(0, 0, 28);

            // Center GT Emblem Text
            canvas.FontColor = Color.FromArgb("#FFD700");
            canvas.FontSize = 14;
            canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.DrawString("GT", -14, -10, 28, 20, HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.RestoreState();
        }

        private void DrawMinimapRadar(ICanvas canvas, RectF bounds)
        {
            // Positioned at top-left HUD (Circular Minimap Radar - Safe Off-Road Position)
            float cx = 120f;
            float cy = 80f;
            float radius = 48f;

            canvas.SaveState();
            canvas.Translate(cx, cy);

            // Dark Translucent Radar Disk
            canvas.FillColor = Color.FromRgba(10, 14, 23, 220);
            canvas.FillCircle(0, 0, radius);
            canvas.StrokeColor = Color.FromArgb("#00E5FF");
            canvas.StrokeSize = 2.5f;
            canvas.DrawCircle(0, 0, radius);

            // Dynamic Outer Circular Fuel Gauge Ring (Green > 50%, Orange < 50%, Red < 20%)
            float fuelPct = Math.Clamp(_engine.Fuel / _engine.MaxFuel, 0f, 1f);
            Color fuelRingColor = fuelPct > 0.50f ? Color.FromArgb("#00FF88") :
                                 fuelPct > 0.20f ? Color.FromArgb("#FF9900") : Color.FromArgb("#FF1A40");

            float fuelRingRadius = radius + 4f;
            canvas.StrokeColor = Color.FromRgba(30, 41, 59, 200);
            canvas.StrokeSize = 4f;
            canvas.DrawCircle(0, 0, fuelRingRadius);

            if (fuelPct > 0.01f)
            {
                canvas.StrokeColor = fuelRingColor;
                canvas.StrokeSize = 4.5f;
                float sweepAngle = fuelPct * 360f;
                canvas.DrawArc(-fuelRingRadius, -fuelRingRadius, fuelRingRadius * 2f, fuelRingRadius * 2f, -90f, -90f + sweepAngle, true, false);
            }

            // Concentric Radar Grid Rings & Crosshair Lines
            canvas.StrokeColor = Color.FromRgba(0, 229, 255, 60);
            canvas.StrokeSize = 1f;
            canvas.DrawCircle(0, 0, radius * 0.6f);
            canvas.DrawLine(-radius * 0.8f, 0, radius * 0.8f, 0);
            canvas.DrawLine(0, -radius * 0.8f, 0, radius * 0.8f);

            // Traffic & Obstacle Radar Blips
            foreach (var obs in _engine.Obstacles)
            {
                if (!obs.IsActive) continue;
                float relY = (obs.Y - _engine.PlayerY) * 0.18f;
                float relX = (obs.X - _engine.PlayerX) * 0.18f;
                if ((relX * relX + relY * relY) < (radius - 6) * (radius - 6))
                {
                    canvas.FillColor = Color.FromArgb("#FF1A40");
                    canvas.FillCircle(relX, relY, 3);
                }
            }

            // Gas & Nitro Collectible Radar Blips (Gold/Cyan)
            foreach (var col in _engine.Collectibles)
            {
                if (!col.IsActive) continue;
                float relY = (col.Y - _engine.PlayerY) * 0.18f;
                float relX = (col.X - _engine.PlayerX) * 0.18f;
                if ((relX * relX + relY * relY) < (radius - 6) * (radius - 6))
                {
                    canvas.FillColor = col.Type == CollectibleType.NitroTank ? Color.FromArgb("#00E5FF") : Color.FromArgb("#FFD700");
                    canvas.FillCircle(relX, relY, 2.5f);
                }
            }

            // Dynamic Road Line Trajectory Direction Indicator (Glowing Cyan/Green)
            canvas.StrokeColor = Color.FromArgb("#00FF88");
            canvas.StrokeSize = 2.5f;
            float steerRad = _engine.SteeringAngle * (float)Math.PI / 180f;
            float lineLen = 18f;
            float endX = (float)Math.Sin(steerRad) * lineLen;
            float endY = -(float)Math.Cos(steerRad) * lineLen;

            canvas.DrawLine(0, 0, endX, endY);
            canvas.FillColor = Color.FromArgb("#00FF88");
            canvas.FillCircle(endX, endY, 3.5f);

            canvas.RestoreState();
        }
    }
}
