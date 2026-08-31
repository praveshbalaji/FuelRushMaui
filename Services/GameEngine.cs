using System;
using System.Collections.Generic;
using System.Linq;
using FuelRushMaui.Models;

namespace FuelRushMaui.Services
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }

    public enum GameOverType
    {
        None,
        Crash,
        OutOfFuel,
        LevelComplete
    }

    public class GameEngine
    {
        public event Action? OnStateChanged;
        public event Action? OnFrameTick;

        private readonly Random _random = new();
        private readonly StorageService _storageService;
        private readonly GarageService _garageService;
        private readonly SoundService _soundService;

        // State
        public GameState State { get; private set; } = GameState.Menu;
        public GameOverType GameOverReason { get; private set; } = GameOverType.None;
        public Vehicle CurrentVehicle { get; private set; } = null!;

        // Level Progression
        public int CurrentLevel { get; set; } = 1;
        public float TargetDistanceKm => 1.5f + (CurrentLevel * 1.0f);

        // Widescreen Landscape Canvas & Road Geometry (5 Lanes)
        public float CanvasWidth { get; set; } = 1080f;
        public float CanvasHeight { get; set; } = 540f;
        public int LaneCount => 5;
        public float RoadWidth => Math.Min(CanvasWidth * 0.88f, 920f);
        public float RoadLeft => (CanvasWidth - RoadWidth) / 2f;
        public float RoadRight => RoadLeft + RoadWidth;
        public float LaneWidth => RoadWidth / LaneCount;

        // Road Scrolling
        public float RoadScrollY { get; set; }

        // Player Position & Driving Controls
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }
        public float PlayerWidth { get; set; } = 52f;
        public float PlayerHeight { get; set; } = 96f;

        // Real-Time Interactive Steering Wheel (-45 deg left to +45 deg right)
        public float SteeringAngle { get; set; } = 0f;
        public float TargetSteeringAngle { get; set; } = 0f;
        public float NormalizedSteerInput { get; set; } = 0f; // -1.0 (Left) to +1.0 (Right)

        // Pedals Physics
        public bool IsGasPressed { get; set; } = false;
        public bool IsBrakePressed { get; set; } = false;

        // RPM & Tachometer Physics
        public float CurrentRpm { get; set; } = 1000f;
        public int CurrentGear { get; set; } = 1;

        // Speed & Performance
        public float CurrentSpeedKmH { get; set; } = 0f;
        public float BaseSpeed { get; set; } = 8.0f;
        public float MaxSpeed { get; set; } = 18f;

        // Fuel & Nitro
        public float Fuel { get; set; } = 100f;
        public float MaxFuel { get; set; } = 100f;
        public float FuelDrainRate { get; set; } = 0.02f; // Forgiving, child-friendly baseline drain rate
        public bool IsCoasting => !IsGasPressed && CurrentSpeedKmH > 10f;

        public float Nitro { get; set; } = 40f;
        public bool IsNitroActive { get; set; } = false;
        public float NitroTimer { get; set; } = 0f;

        public bool IsShieldActive { get; set; } = false;
        public float ShieldTimer { get; set; } = 0f;

        public int Score { get; set; }
        public float DistanceKm { get; set; }
        public int CoinsCollectedInRun { get; set; }
        public int Multiplier { get; set; } = 1;
        public float MultiplierTimer { get; set; } = 0f;

        // VFX Camera Shake
        public float CameraShakeX { get; set; } = 0f;
        public float CameraShakeY { get; set; } = 0f;

        // Entities
        public List<Collectible> Collectibles { get; } = new();
        public List<Obstacle> Obstacles { get; } = new();
        public List<Particle> Particles { get; } = new();

        // Timers
        private float _collectibleSpawnTimer;
        private float _obstacleSpawnTimer;
        private float _difficultyTimer;

        // AI Co-Pilot Telemetry & Dynamic Difficulty Engine
        public AICoPilotService AICoPilot { get; } = new();

        public GameEngine(StorageService storageService, GarageService garageService, SoundService soundService)
        {
            _storageService = storageService;
            _garageService = garageService;
            _soundService = soundService;
            CurrentVehicle = _garageService.GetSelectedVehicle();
        }

        public void StartGame()
        {
            DeactivateNitro();

            CurrentVehicle = _garageService.GetSelectedVehicle();
            _soundService.StartTokyoDriftBgm();

            Fuel = 100f;
            MaxFuel = 100f;
            Nitro = 0f; // Finite Nitro: starts at 0, must collect NitroTank pickups
            Score = 0;
            DistanceKm = 0f;
            CoinsCollectedInRun = 0;
            Multiplier = 1;
            MultiplierTimer = 0f;
            GameOverReason = GameOverType.None;

            CurrentSpeedKmH = 0f;
            CurrentRpm = 1000f;
            CurrentGear = 1;
            CameraShakeX = 0f;
            CameraShakeY = 0f;
            RoadScrollY = 0f;

            IsNitroActive = false;
            IsShieldActive = false;
            ShieldTimer = 0f;
            NitroTimer = 0f;

            IsGasPressed = true; // Auto gas start
            IsBrakePressed = false;
            SteeringAngle = 0f;
            TargetSteeringAngle = 0f;
            NormalizedSteerInput = 0f;

            PlayerX = CanvasWidth > 0 ? CanvasWidth / 2f : 540f;
            PlayerY = CanvasHeight > 0 ? CanvasHeight * 0.76f : 410f;

            Collectibles.Clear();
            Obstacles.Clear();
            Particles.Clear();

            _collectibleSpawnTimer = 0.6f;
            _obstacleSpawnTimer = 1.0f;
            _difficultyTimer = 0f;

            State = GameState.Playing;
            OnStateChanged?.Invoke();
        }

        public void RestartGame()
        {
            StartGame();
        }

        public void PauseGame()
        {
            if (State == GameState.Playing)
            {
                State = GameState.Paused;
                OnStateChanged?.Invoke();
            }
            else if (State == GameState.Paused)
            {
                State = GameState.Playing;
                OnStateChanged?.Invoke();
            }
        }

        public void EndGame(GameOverType reason)
        {
            if (State == GameState.GameOver) return;

            GameOverReason = reason;
            State = GameState.GameOver;

            if (reason == GameOverType.Crash)
            {
                _soundService.PlayCrash();
                CreateExplosionParticles(PlayerX, PlayerY, 35);
            }
            else if (reason == GameOverType.LevelComplete)
            {
                CoinsCollectedInRun += 500; // Bonus for level completion
                _soundService.PlayNitroBoost();
            }
            else if (reason == GameOverType.OutOfFuel)
            {
                _soundService.PlayLowFuelAlert();
            }

            _storageService.AddCoins(CoinsCollectedInRun);
            _storageService.UpdateMaxSpeedKmH(CurrentSpeedKmH);
            _storageService.UpdateMaxLevelReached(CurrentLevel);
            if (reason == GameOverType.LevelComplete)
            {
                _storageService.IncrementGasStationsReached();
                _storageService.UpdateMaxLevelReached(CurrentLevel + 1);
            }

            _storageService.SaveHighScore(new HighScore
            {
                Score = Score,
                DistanceKm = (float)Math.Round(DistanceKm, 2),
                CoinsCollected = CoinsCollectedInRun,
                Timestamp = DateTime.Now
            });

            OnStateChanged?.Invoke();
        }

        public bool IsDriftModeActive { get; set; } = false;
        public bool IsSuspensionActive { get; set; } = false;

        public void SetSteeringNormalized(float normValue)
        {
            NormalizedSteerInput = Math.Clamp(normValue, -1.0f, 1.0f);
            TargetSteeringAngle = NormalizedSteerInput * 90f; // Truck/Car simulator 90 degree steering rotation
        }

        public void SetSelectedVehicle(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                CurrentVehicle = vehicle;
                _storageService.SetSelectedVehicleId(vehicle.Id);
                OnStateChanged?.Invoke();
            }
        }

        private System.Threading.CancellationTokenSource? _nitroCts;

        /// <summary>
        /// Activates Nitro boost for exactly 5 seconds using a strict Task.Delay timer
        /// combined with delta-time tick checks, preventing infinite boost hacks.
        /// </summary>
        public async void ActivateNitro()
        {
            if (State != GameState.Playing) return;
            if (IsNitroActive || Nitro < 25f) return;

            // Deduct Nitro energy tank (requires 25% minimum per activation)
            Nitro = Math.Max(0f, Nitro - 25f);
            IsNitroActive = true;
            NitroTimer = 5.0f; // Strict 5.0s duration countdown
            _soundService.PlayNitroBoost();

            // Cancel any existing running timer
            _nitroCts?.Cancel();
            _nitroCts = new System.Threading.CancellationTokenSource();
            var token = _nitroCts.Token;

            try
            {
                // Enforce strict 5-second asynchronous timeout
                await System.Threading.Tasks.Task.Delay(5000, token);
                if (!token.IsCancellationRequested && IsNitroActive)
                {
                    DeactivateNitro();
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                // Suppress exception when cancelled by pause, reset, or game over
            }
        }

        /// <summary>
        /// Reverts vehicle to base speed, base fuel consumption, and disables camera shake.
        /// </summary>
        public void DeactivateNitro()
        {
            _nitroCts?.Cancel();
            _nitroCts = null;

            IsNitroActive = false;
            NitroTimer = 0f;
            CameraShakeX = 0f;
            CameraShakeY = 0f;
        }

        public void ShiftGearNext()
        {
            CurrentGear = (CurrentGear % 6) + 1;
            _soundService.PlayEngineRev();
        }

        public void ToggleDriftMode()
        {
            IsDriftModeActive = !IsDriftModeActive;
        }

        public void ToggleSuspension()
        {
            IsSuspensionActive = !IsSuspensionActive;
        }

        public void ResetPlayerCar()
        {
            PlayerX = CanvasWidth / 2f;
            SteeringAngle = 0f;
            TargetSteeringAngle = 0f;
            NormalizedSteerInput = 0f;
        }

        public void ReturnToMenu()
        {
            State = GameState.Menu;
            SteeringAngle = 0f;
            TargetSteeringAngle = 0f;
            NormalizedSteerInput = 0f;
            CurrentSpeedKmH = 0f;
            IsGasPressed = false;
            IsBrakePressed = false;
            IsNitroActive = false;
            OnStateChanged?.Invoke();
        }

        public float GetLaneCenterX(int laneIndex)
        {
            laneIndex = Math.Clamp(laneIndex, 0, LaneCount - 1);
            return RoadLeft + (laneIndex + 0.5f) * LaneWidth;
        }

        public void Update(float deltaTime)
        {
            if (State != GameState.Playing)
            {
                UpdateParticles(deltaTime);
                return;
            }

            _difficultyTimer += deltaTime;

            // 1. Steering Wheel Interpolation & Real-Time Steering Movement
            SteeringAngle += (TargetSteeringAngle - SteeringAngle) * Math.Min(1.0f, 15f * deltaTime);
            float maxMoveSpeed = 480f * CurrentVehicle.Handling;
            PlayerX += NormalizedSteerInput * maxMoveSpeed * deltaTime;
            PlayerX = Math.Clamp(PlayerX, RoadLeft + PlayerWidth / 2f + 10f, RoadRight - PlayerWidth / 2f - 10f);

            // 2. Pedals & Speed Calculations
            float targetSpeed = BaseSpeed * CurrentVehicle.TopSpeed;

            if (IsGasPressed)
            {
                targetSpeed *= 1.35f;
            }
            if (IsBrakePressed)
            {
                targetSpeed = 0f; // Complete 0 km/h stop
                // Emit brake tire smoke
                if (CurrentSpeedKmH > 30f && _random.NextDouble() < 0.4)
                {
                    CreateSmokeParticles(PlayerX - 16, PlayerY + PlayerHeight / 2f);
                    CreateSmokeParticles(PlayerX + 16, PlayerY + PlayerHeight / 2f);
                }
            }
            if (IsNitroActive)
            {
                NitroTimer -= deltaTime;
                targetSpeed *= 1.85f;
                CreateNitroParticles(PlayerX, PlayerY + PlayerHeight * 0.4f);

                // Nitro Camera Shake
                CameraShakeX = (float)((_random.NextDouble() - 0.5) * 6.0);
                CameraShakeY = (float)((_random.NextDouble() - 0.5) * 6.0);

                if (NitroTimer <= 0f)
                {
                    DeactivateNitro();
                }
            }
            else
            {
                CameraShakeX = 0f;
                CameraShakeY = 0f;
            }

            // Smooth speed transition
            CurrentSpeedKmH += (targetSpeed * 18.5f - CurrentSpeedKmH) * Math.Min(1.0f, 6.0f * deltaTime);
            if (CurrentSpeedKmH < 0.5f) CurrentSpeedKmH = 0f;

            // RPM & Gear Physics
            float maxRpm = CurrentVehicle.MaxRpm;
            float rpmRatio = (CurrentSpeedKmH / (CurrentVehicle.TopSpeed * 350f));
            CurrentRpm = Math.Clamp(1200f + rpmRatio * (maxRpm - 1200f), 1000f, maxRpm);
            CurrentGear = Math.Clamp((int)(1 + CurrentSpeedKmH / 65f), 1, 6);

            // Scroll Road
            float actualScrollSpeed = (CurrentSpeedKmH / 18.5f);
            RoadScrollY = (RoadScrollY + actualScrollSpeed) % 90f;

            // Distance & Score
            float deltaKm = (actualScrollSpeed * deltaTime) / 140f;
            DistanceKm += deltaKm;
            Score += (int)(actualScrollSpeed * 2.0f * Multiplier);

            // Check Gas Station Level Target Arrival
            if (DistanceKm >= TargetDistanceKm)
            {
                EndGame(GameOverType.LevelComplete);
                return;
            }

            // Child-Friendly Educational Fuel Conservation Physics & Coasting Mechanic
            float speedRatio = Math.Max(0f, CurrentSpeedKmH / (CurrentVehicle.TopSpeed * 18.5f));
            float accelFactor = IsGasPressed ? 1.0f : (CurrentSpeedKmH > 5f ? 0.05f : 0.0f); // Coasting = 95% fuel savings!
            float nitroSavingsFactor = IsNitroActive ? 0.30f : 1.0f; // Jackpot: 70% fuel savings per km
            float speedBasedDrain = speedRatio * FuelDrainRate * accelFactor * (1.0f / Math.Max(0.5f, CurrentVehicle.FuelEfficiency)) * nitroSavingsFactor;
            Fuel -= speedBasedDrain * (deltaTime * 30f);

            if (Fuel <= 0f)
            {
                Fuel = 0f;
                EndGame(GameOverType.OutOfFuel);
                return;
            }

            if (Fuel < 20f && _random.NextDouble() < 0.05)
            {
                _soundService.PlayLowFuelAlert();
            }

            // Timers & Spawning
            if (IsShieldActive)
            {
                ShieldTimer -= deltaTime;
                if (ShieldTimer <= 0f) IsShieldActive = false;
            }

            if (Multiplier > 1)
            {
                MultiplierTimer -= deltaTime;
                if (MultiplierTimer <= 0f) Multiplier = 1;
            }

            UpdateSpawning(deltaTime, actualScrollSpeed);
            UpdateCollectibles(actualScrollSpeed);
            UpdateObstacles(actualScrollSpeed);
            UpdateParticles(deltaTime);
            CheckCollisions();

            // Process AI Co-Pilot Telemetry Frame
            float minObsDist = Obstacles.Where(o => o.IsActive && o.Y < PlayerY).Select(o => PlayerY - o.Y).DefaultIfEmpty(999f).Min();
            AICoPilot.ProcessTelemetry(CurrentSpeedKmH, NormalizedSteerInput, IsBrakePressed, minObsDist, deltaTime);

            OnFrameTick?.Invoke();
        }

        private void UpdateSpawning(float deltaTime, float speed)
        {
            _collectibleSpawnTimer -= deltaTime;
            if (_collectibleSpawnTimer <= 0f)
            {
                _collectibleSpawnTimer = (float)(0.9 + _random.NextDouble() * 1.2);
                SpawnCollectible();
            }

            _obstacleSpawnTimer -= deltaTime;
            if (_obstacleSpawnTimer <= 0f)
            {
                float dda = AICoPilot.LastPrediction.DynamicDifficultyMultiplier;
                float baseInterval = Math.Max(0.45f, (1.4f - (_difficultyTimer * 0.01f)) / dda);
                _obstacleSpawnTimer = (float)(baseInterval + _random.NextDouble() * 0.5);
                SpawnObstacle();
            }
        }

        private void SpawnCollectible()
        {
            int lane = _random.Next(0, LaneCount);
            float spawnX = GetLaneCenterX(lane);

            double roll = _random.NextDouble();
            CollectibleType type;

            if (Fuel < 40f || roll < 0.35)
            {
                type = CollectibleType.FuelCanister;
            }
            else if (roll < 0.75)
            {
                type = CollectibleType.GoldCoin;
            }
            else if (roll < 0.88)
            {
                type = CollectibleType.NitroTank;
            }
            else if (roll < 0.95)
            {
                type = CollectibleType.EnergyShield;
            }
            else
            {
                type = CollectibleType.DoubleMultiplier;
            }

            Collectibles.Add(new Collectible
            {
                X = spawnX,
                Y = -50f,
                LaneIndex = lane,
                Type = type
            });
        }

        private void SpawnObstacle()
        {
            int lane = _random.Next(0, LaneCount);
            float spawnX = GetLaneCenterX(lane);

            if (Collectibles.Any(c => c.LaneIndex == lane && c.Y < 90f)) return;

            double typeRoll = _random.NextDouble();
            ObstacleType type;
            float width = 50f;
            float height = 90f;
            string colorHex = "#3399FF";
            float speedY = (float)(2.2 + _random.NextDouble() * 2.8);

            if (typeRoll < 0.35)
            {
                type = ObstacleType.SedanCar;
                colorHex = "#FF3333";
            }
            else if (typeRoll < 0.60)
            {
                type = ObstacleType.DeliveryTruck;
                width = 58f;
                height = 120f;
                colorHex = "#FFBB00";
                speedY = 1.8f;
            }
            else if (typeRoll < 0.80)
            {
                type = ObstacleType.PoliceCar;
                colorHex = "#111111";
                speedY = 4.0f;
            }
            else if (typeRoll < 0.92)
            {
                type = ObstacleType.OilSlick;
                width = 54f;
                height = 42f;
                colorHex = "#1B1F2D";
                speedY = 0f;
            }
            else
            {
                type = ObstacleType.RoadBarrier;
                width = 48f;
                height = 38f;
                colorHex = "#FF5500";
                speedY = 0f;
            }

            Obstacles.Add(new Obstacle
            {
                X = spawnX,
                Y = -140f,
                LaneIndex = lane,
                Width = width,
                Height = height,
                Type = type,
                ColorHex = colorHex,
                SpeedY = speedY
            });
        }

        private void UpdateCollectibles(float speed)
        {
            for (int i = Collectibles.Count - 1; i >= 0; i--)
            {
                var item = Collectibles[i];
                item.Y += speed;
                item.Rotation += 4f;

                if (item.Y > CanvasHeight + 60f)
                {
                    Collectibles.RemoveAt(i);
                }
            }
        }

        private void UpdateObstacles(float speed)
        {
            for (int i = Obstacles.Count - 1; i >= 0; i--)
            {
                var obs = Obstacles[i];
                obs.Y += (speed - obs.SpeedY);

                if (obs.Y > CanvasHeight + 140f)
                {
                    Obstacles.RemoveAt(i);
                }
            }
        }

        private void UpdateParticles(float deltaTime)
        {
            for (int i = Particles.Count - 1; i >= 0; i--)
            {
                var p = Particles[i];
                p.X += p.VX;
                p.Y += p.VY;
                p.Life -= deltaTime * 1.8f;

                if (p.Life <= 0f)
                {
                    Particles.RemoveAt(i);
                }
            }
        }

        private void CheckCollisions()
        {
            float pLeft = PlayerX - PlayerWidth / 2f;
            float pRight = PlayerX + PlayerWidth / 2f;
            float pTop = PlayerY - PlayerHeight / 2f;
            float pBottom = PlayerY + PlayerHeight / 2f;

            for (int i = Collectibles.Count - 1; i >= 0; i--)
            {
                var c = Collectibles[i];
                if (!c.IsActive) continue;

                float dist = (float)Math.Sqrt(Math.Pow(PlayerX - c.X, 2) + Math.Pow(PlayerY - c.Y, 2));
                if (dist < (c.Radius + PlayerWidth / 2f))
                {
                    c.IsActive = false;
                    ApplyCollectibleEffect(c);
                    CreateSparkParticles(c.X, c.Y, "#00FFDD", 12);
                    Collectibles.RemoveAt(i);
                }
            }

            for (int i = Obstacles.Count - 1; i >= 0; i--)
            {
                var obs = Obstacles[i];
                if (!obs.IsActive) continue;

                float oLeft = obs.X - obs.Width / 2f;
                float oRight = obs.X + obs.Width / 2f;
                float oTop = obs.Y - obs.Height / 2f;
                float oBottom = obs.Y + obs.Height / 2f;

                if (pLeft < oRight && pRight > oLeft && pTop < oBottom && pBottom > oTop)
                {
                    if (obs.Type == ObstacleType.OilSlick)
                    {
                        Fuel -= 10f;
                        NormalizedSteerInput = (float)(_random.NextDouble() > 0.5 ? 0.8 : -0.8);
                        CreateSparkParticles(PlayerX, PlayerY, "#336699", 14);
                        obs.IsActive = false;
                    }
                    else if (IsNitroActive || IsShieldActive)
                    {
                        obs.IsActive = false;
                        CreateExplosionParticles(obs.X, obs.Y, 18);
                        Score += 600;
                        _soundService.PlayCrash();
                    }
                    else
                    {
                        EndGame(GameOverType.Crash);
                        return;
                    }
                }
            }
        }

        private void ApplyCollectibleEffect(Collectible c)
        {
            switch (c.Type)
            {
                case CollectibleType.FuelCanister:
                    Fuel = Math.Min(MaxFuel, Fuel + 35f);
                    Score += 300;
                    _soundService.PlayFuelPickup();
                    break;
                case CollectibleType.GoldCoin:
                    CoinsCollectedInRun++;
                    Score += 200;
                    _soundService.PlayCoinPickup();
                    break;
                case CollectibleType.NitroTank:
                    Nitro = Math.Min(100f, Nitro + 40f);
                    Score += 350;
                    _soundService.PlayNitroBoost();
                    break;
                case CollectibleType.EnergyShield:
                    IsShieldActive = true;
                    ShieldTimer = 7.0f;
                    _soundService.PlayShieldPickup();
                    break;
                case CollectibleType.DoubleMultiplier:
                    Multiplier = 2;
                    MultiplierTimer = 10.0f;
                    _soundService.PlayCoinPickup();
                    break;
            }
        }

        private void CreateSparkParticles(float x, float y, string colorHex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = (float)(_random.NextDouble() * Math.PI * 2);
                float speed = (float)(2 + _random.NextDouble() * 5);
                Particles.Add(new Particle
                {
                    X = x,
                    Y = y,
                    VX = (float)Math.Cos(angle) * speed,
                    VY = (float)Math.Sin(angle) * speed,
                    Life = 1.0f,
                    Size = (float)(4 + _random.NextDouble() * 5),
                    ColorHex = colorHex,
                    Type = ParticleType.Spark
                });
            }
        }

        private void CreateExplosionParticles(float x, float y, int count)
        {
            string[] colors = { "#FF3300", "#FF9900", "#FFFF00", "#222222" };
            for (int i = 0; i < count; i++)
            {
                float angle = (float)(_random.NextDouble() * Math.PI * 2);
                float speed = (float)(3 + _random.NextDouble() * 9);
                Particles.Add(new Particle
                {
                    X = x,
                    Y = y,
                    VX = (float)Math.Cos(angle) * speed,
                    VY = (float)Math.Sin(angle) * speed,
                    Life = 1.0f,
                    Size = (float)(6 + _random.NextDouble() * 10),
                    ColorHex = colors[_random.Next(colors.Length)],
                    Type = ParticleType.Explosion
                });
            }
        }

        private void CreateNitroParticles(float x, float y)
        {
            for (int i = 0; i < 4; i++)
            {
                Particles.Add(new Particle
                {
                    X = x + (float)((_random.NextDouble() - 0.5) * 20),
                    Y = y,
                    VX = (float)((_random.NextDouble() - 0.5) * 3),
                    VY = (float)(5 + _random.NextDouble() * 7),
                    Life = 0.8f,
                    Size = (float)(6 + _random.NextDouble() * 8),
                    ColorHex = _random.NextDouble() > 0.4 ? "#00E5FF" : "#00FFDD",
                    Type = ParticleType.NitroFlame
                });
            }
        }

        private void CreateSmokeParticles(float x, float y)
        {
            Particles.Add(new Particle
            {
                X = x,
                Y = y,
                VX = (float)((_random.NextDouble() - 0.5) * 4),
                VY = (float)(2 + _random.NextDouble() * 3),
                Life = 0.9f,
                Size = (float)(8 + _random.NextDouble() * 8),
                ColorHex = "#888888",
                Type = ParticleType.Smoke
            });
        }
    }
}
