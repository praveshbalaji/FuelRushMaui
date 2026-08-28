using System;
using FuelRushMaui.Renderers;
using FuelRushMaui.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;

namespace FuelRushMaui
{
    public partial class MainPage : ContentPage
    {
        private readonly StorageService _storageService;
        private readonly GarageService _garageService;
        private readonly SoundService _soundService;
        private readonly GameEngine _gameEngine;
        private readonly GameCanvasDrawable _canvasDrawable;

        private IDispatcherTimer? _gameLoopTimer;
        private DateTime _lastFrameTime;

        public MainPage()
        {
            InitializeComponent();

            _storageService = new StorageService();
            _garageService = new GarageService(_storageService);
            _soundService = new SoundService(_storageService);

            _gameEngine = new GameEngine(_storageService, _garageService, _soundService);
            _canvasDrawable = new GameCanvasDrawable(_gameEngine);

            gameGraphicsView.Drawable = _canvasDrawable;

            _gameEngine.OnStateChanged += HandleStateChanged;

            App.OnAppSleeping += HandleAppSleeping;
            App.OnAppResuming += HandleAppResuming;

            UpdateMenuUI();
            SetupGameLoop();
        }

        private void HandleAppSleeping()
        {
            if (_gameEngine.State == GameState.Playing)
            {
                _gameEngine.PauseGame();
            }
        }

        private void HandleAppResuming()
        {
            _lastFrameTime = DateTime.Now;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                // Force Landscape orientation
                DeviceDisplay.Current.MainDisplayInfoChanged += OnDisplayInfoChanged;
            }
            catch
            {
                // Fallback gracefully
            }
        }

        private void OnDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
        {
            // Lock orientation logic if required by platform
        }

        private void SetupGameLoop()
        {
            _gameLoopTimer = Dispatcher.CreateTimer();
            _gameLoopTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            _gameLoopTimer.Tick += GameLoopTick;
            _lastFrameTime = DateTime.Now;
            _gameLoopTimer.Start();
        }

        private void GameLoopTick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            float deltaTime = (float)(now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;

            deltaTime = Math.Min(0.05f, deltaTime);

            _gameEngine.Update(deltaTime);
            gameGraphicsView.Invalidate();

            if (_gameEngine.State == GameState.Playing)
            {
                UpdateHUD();
            }
        }

        private void UpdateHUD()
        {
            lblScore.Text = _gameEngine.Score.ToString("N0");
            lblDistance.Text = $"{_gameEngine.DistanceKm:0.0} km";
            lblTargetDistance.Text = $"{_gameEngine.DistanceKm:0.0} / {_gameEngine.TargetDistanceKm:0.0} KM";
            lblSpeed.Text = $"{_gameEngine.CurrentSpeedKmH:0}";
            lblCoinsHUD.Text = _gameEngine.CoinsCollectedInRun.ToString();
            badgeCoasting.IsVisible = _gameEngine.IsCoasting;

            float fuelPct = Math.Clamp(_gameEngine.Fuel / _gameEngine.MaxFuel, 0f, 1f);
            pbFuelGauge.Progress = fuelPct;
            lblFuelPct.Text = $"{fuelPct * 100:0}%";

            if (fuelPct < 0.25f)
            {
                pbFuelGauge.ProgressColor = Color.FromArgb("#FF1A40");
            }
            else if (fuelPct < 0.50f)
            {
                pbFuelGauge.ProgressColor = Color.FromArgb("#FFD700");
            }
            else
            {
                pbFuelGauge.ProgressColor = Color.FromArgb("#00FF88");
            }

            btnNitro.IsEnabled = (_gameEngine.Nitro >= 25f && !_gameEngine.IsNitroActive);
            btnNitro.Opacity = btnNitro.IsEnabled ? 1.0 : (_gameEngine.IsNitroActive ? 0.9 : 0.4);
            btnNitro.Text = _gameEngine.IsNitroActive ? "⚡ ACTIVE" : (_gameEngine.Nitro >= 25f ? "⚡ NOS" : $"⚡ {_gameEngine.Nitro:0}%");
        }

        private void UpdateMenuUI()
        {
            var selected = _garageService.GetSelectedVehicle();
            lblMenuVehicle.Text = $"{selected.Name}";
            if (imgMenuVehicle != null)
            {
                imgMenuVehicle.Source = selected.ImagePath;
            }
            btnSoundToggle.Text = _storageService.IsSoundEnabled() ? "🔊 AUDIO: ON" : "🔇 AUDIO: OFF";
        }

        private void HandleStateChanged()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (_gameEngine.State)
                {
                    case GameState.Menu:
                        hudLayer.IsVisible = false;
                        menuOverlay.IsVisible = true;
                        pauseOverlay.IsVisible = false;
                        gameOverOverlay.IsVisible = false;
                        UpdateMenuUI();
                        break;

                    case GameState.Playing:
                        hudLayer.IsVisible = true;
                        menuOverlay.IsVisible = false;
                        pauseOverlay.IsVisible = false;
                        gameOverOverlay.IsVisible = false;
                        break;

                    case GameState.Paused:
                        hudLayer.IsVisible = true;
                        menuOverlay.IsVisible = false;
                        pauseOverlay.IsVisible = true;
                        gameOverOverlay.IsVisible = false;
                        break;

                    case GameState.GameOver:
                        hudLayer.IsVisible = false;
                        menuOverlay.IsVisible = false;
                        pauseOverlay.IsVisible = false;
                        gameOverOverlay.IsVisible = true;

                        lblGameOverScore.Text = _gameEngine.Score.ToString("N0");
                        lblGameOverDist.Text = $"{_gameEngine.DistanceKm:0.0} km";
                        lblGameOverCoins.Text = $"+{_gameEngine.CoinsCollectedInRun}";

                        if (_gameEngine.GameOverReason == GameOverType.Crash)
                        {
                            lblGameOverTitle.Text = "💥 CAR CRASHED!";
                            lblGameOverTitle.TextColor = Color.FromArgb("#FF1A40");
                        }
                        else if (_gameEngine.GameOverReason == GameOverType.LevelComplete)
                        {
                            lblGameOverTitle.Text = "⛽ GAS STATION REACHED!";
                            lblGameOverTitle.TextColor = Color.FromArgb("#00FF88");
                            _gameEngine.CurrentLevel++;
                        }
                        else
                        {
                            lblGameOverTitle.Text = "⛽ OUT OF FUEL!";
                            lblGameOverTitle.TextColor = Color.FromArgb("#FF9900");
                        }
                        break;
                }
            });
        }

        // --- Real-Time Simulator Steering Pan Gesture & Pedal Handlers ---

        // --- Real-Time Simulator Steering Pan Gesture & Pedal Handlers ---

        private double _wheelCurrentRotation = 0;
        private bool _isPanning = false;

        private void OnSteeringWheelPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _isPanning = true;
                    this.AbortAnimation("SteeringWheelReturn");
                    break;

                case GestureStatus.Running:
                    _isPanning = true;
                    // Convert touch drag displacement (TotalX) into rotational angle (-90 to +90 degrees)
                    double targetRotation = Math.Clamp(e.TotalX * 1.0, -90.0, 90.0);
                    _wheelCurrentRotation = targetRotation;

                    // Rotate single steering wheel UI element in real time
                    if (imgSteeringWheel != null)
                    {
                        imgSteeringWheel.Rotation = _wheelCurrentRotation;
                    }

                    // Continuous floating-point steering mapping (-1.0f to 1.0f) for game physics loop
                    float normalizedSteer = (float)Math.Clamp(_wheelCurrentRotation / 90.0, -1.0, 1.0);
                    _gameEngine.SetSteeringNormalized(normalizedSteer);
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    _isPanning = false;
                    // Smooth analog return-to-center logic (prevents instant snapping)
                    AnimateSteeringReturnToCenter();
                    break;
            }
        }

        private void AnimateSteeringReturnToCenter()
        {
            this.AbortAnimation("SteeringWheelReturn");

            var animate = new Animation(v =>
            {
                if (!_isPanning)
                {
                    _wheelCurrentRotation = v;
                    if (imgSteeringWheel != null)
                    {
                        imgSteeringWheel.Rotation = _wheelCurrentRotation;
                    }
                    float normalizedSteer = (float)Math.Clamp(_wheelCurrentRotation / 90.0, -1.0, 1.0);
                    _gameEngine.SetSteeringNormalized(normalizedSteer);
                }
            }, _wheelCurrentRotation, 0, Easing.CubicOut);

            animate.Commit(this, "SteeringWheelReturn", 16, 250, Easing.CubicOut);
        }

        private void OnGasPressed(object sender, EventArgs e)
        {
            _gameEngine.IsGasPressed = true;
        }

        private void OnGasReleased(object sender, EventArgs e)
        {
            _gameEngine.IsGasPressed = false;
        }

        private void OnBrakePressed(object sender, EventArgs e)
        {
            _gameEngine.IsBrakePressed = true;
        }

        private void OnBrakeReleased(object sender, EventArgs e)
        {
            _gameEngine.IsBrakePressed = false;
        }

        private void OnNitroClicked(object sender, EventArgs e)
        {
            _gameEngine.ActivateNitro();
        }

        private void OnGearShiftClicked(object sender, EventArgs e)
        {
            _gameEngine.ShiftGearNext();
        }

        private void OnDriftModeClicked(object sender, EventArgs e)
        {
            _gameEngine.ToggleDriftMode();
        }

        private void OnSuspensionClicked(object sender, EventArgs e)
        {
            _gameEngine.ToggleSuspension();
        }

        private void OnResetCarClicked(object sender, EventArgs e)
        {
            _gameEngine.ResetPlayerCar();
        }

        private void OnCameraSwitchClicked(object sender, EventArgs e)
        {
            // Toggle camera perspective view
        }

        private void OnPlayClicked(object sender, EventArgs e)
        {
            _gameEngine.StartGame();
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            _gameEngine.PauseGame();
        }

        private void OnReplayClicked(object sender, EventArgs e)
        {
            _gameEngine.StartGame();
        }

        private void OnQuitToMenuClicked(object sender, EventArgs e)
        {
            _gameEngine.ReturnToMenu();
            pauseOverlay.IsVisible = false;
            gameOverOverlay.IsVisible = false;
            hudLayer.IsVisible = false;
            menuOverlay.IsVisible = true;
            UpdateMenuUI();
        }

        private void OnGarageClicked(object sender, EventArgs e)
        {
            garageModal.Initialize(_garageService, _storageService, _gameEngine);
            garageModal.IsVisible = true;
        }

        private void OnLeaderboardClicked(object sender, EventArgs e)
        {
            scoresModal.LoadData(_storageService);
            scoresModal.IsVisible = true;
        }

        private void OnSoundToggleClicked(object sender, EventArgs e)
        {
            bool current = _storageService.IsSoundEnabled();
            _storageService.SetSoundEnabled(!current);
            UpdateMenuUI();
        }

        private void OnModalClosed()
        {
            garageModal.IsVisible = false;
            scoresModal.IsVisible = false;
            UpdateMenuUI();
        }
    }
}
