<div align="center">
  <img src="Resources/Images/game_icon.png" width="140" alt="Fuel Rush Icon" />
  <h1 style="font-family: 'Montserrat', sans-serif; color: #00E5FF; font-size: 38px; margin-top: 10px;">Fuel Rush</h1>
  <p style="font-family: 'Montserrat', sans-serif; color: #94A3B8; font-size: 18px;"><b>Mustang Simulator Apex — .NET MAUI High-Speed 2D Racing Simulator</b></p>

  <p>
    <a href="https://dotnet.microsoft.com/en-us/apps/maui"><img src="https://img.shields.io/badge/.NET%20MAUI-net10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET MAUI" /></a>
    <a href="https://github.com/praveshbalaji/FuelRushMaui/releases"><img src="https://img.shields.io/badge/Platform-Windows%20%7C%20Android%20%7C%20iOS%20%7C%20macOS-113979?style=for-the-badge" alt="Platforms" /></a>
    <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge" alt="License" /></a>
  </p>
</div>

---

<div align="center" style="background: linear-gradient(135deg, #0a0e17 0%, #113979 100%); padding: 30px; border-radius: 16px; margin: 25px 0;">
  <h2 style="color: #FFD700; font-family: 'Montserrat', sans-serif; margin-top: 0; font-size: 26px;">📲 Download Game APK (`FuelRushMaui.apk`)</h2>
  <p style="color: #E2E8F0; font-size: 16px; max-width: 600px; margin: 0 auto 20px auto;">
    Play <b>Fuel Rush</b> directly on your Android phone or PC! Click below to download the latest signed release APK:
  </p>
  
  <p>
    <a href="https://github.com/praveshbalaji/FuelRushMaui/releases/latest/download/FuelRushMaui.apk" style="background-color: #22C55E; color: #FFFFFF; font-family: 'Montserrat', sans-serif; font-weight: bold; font-size: 20px; padding: 16px 36px; text-decoration: none; border-radius: 12px; display: inline-block; box-shadow: 0 4px 14px rgba(34, 197, 94, 0.4);">
      ⚡ DOWNLOAD FUELRUSHMAUI.APK (LATEST SIGNED RELEASE)
    </a>
  </p>
  <p style="color: #94A3B8; font-size: 14px; margin-top: 12px;">
    Alternative direct link: <a href="https://github.com/praveshbalaji/FuelRushMaui/releases/latest/download/fuelrush.apk" style="color: #38BDF8; font-weight: bold;">fuelrush.apk</a> | <a href="https://github.com/praveshbalaji/FuelRushMaui/releases" style="color: #38BDF8;">View All GitHub Releases &amp; Assets</a>
  </p>
</div>

### 📲 Quick Installation Instructions for Android Mobile

1. **Download the APK:** Tap the **`DOWNLOAD FUELRUSHMAUI.APK`** button above directly on your phone.
2. **Allow Installation from Unknown Sources:**
   - If prompted by your browser (Chrome/Files), tap **Settings** and toggle **Allow from this source** to **ON**.
   - Or go to **Settings > Apps > Special App Access > Install Unknown Apps** on your phone.
3. **Bypass Play Protect Warning (Self-Signed APK):**
   - Tap **More details** and then select **Install anyway**.
4. **Already Have Previous Build Installed?**
   - Uninstall older versions of `FuelRushMaui` from your phone first to prevent update signature conflicts.

---

## 🌟 Features & Technical Highlights

- 🏎️ **Dynamic Player Car Synchronization**:
  - The in-game player car directly reflects whichever Mustang model is selected in the Garage.
  - Custom top-down vector models for all **6 Mustang Generations** (1965 Fastback, 1974 Coupe, 1990 Fox Body GT, 2003 SVT Cobra, 2013 Shelby GT500, and 2024 Dark Horse) complete with model-specific shapes, colors, stripes, spoilers, hood scoops, and neon underglow.

- 🎵 **Tokyo Drift Audio Engine**:
  - High-energy **Tokyo Drift** background soundtrack (`tokyo_drift_bgm.wav`) with driving synth bass and drift rhythm.
  - Native Windows multimedia audio integration (`winmm.dll`) and PCM WAV synthesis for real-time sound effects (chimes, nitro boost, crash, engine rev) on Windows and Android.

- ⚡ **Clean Cyber Loader Page**:
  - Custom high-speed Mustang GT loader car graphic with clean alpha-transparent background (`turbo_car_loader.png`).
  - Animated loading progress track with neon green feedback.

- 🎮 **Real-Time Simulator Controls**:
  - Analog 90° center-axis Ford Mustang steering wheel with touch-drag gesture mapping and cubic spring return-to-center physics.
  - Ribbed brake and accelerator pedals with dynamic RPM and tachometer physics.

- 🏆 **Mustang Garage & Achievement System**:
  - Earn coins, reach target distances, and unlock 6 generations of iconic Ford Mustangs through scenario achievement milestones.

---

## 🏎️ Mustang Garage Lineup

<div align="center">
  <img src="Resources/Images/car_mustang_1965.png" width="48%" alt="1965 Mustang" />
  <img src="Resources/Images/car_mustang_2024.png" width="48%" alt="2024 Dark Horse" />
</div>

| # | Generation | Mustang Model | Unlock Achievement Milestone |
|---|---|---|---|
| **1** | **Gen 1 (1965)** | **1965 Mustang Fastback GT** | 🏆 **Scenario 1: Vintage Pioneer** (Default Starter) |
| **2** | **Gen 2 (1974)** | **1974 Mustang II Coupe** | 🏆 **Scenario 2: Gas Station Pioneer** (Reach Level 1) |
| **3** | **Gen 3 (1990)** | **1990 Fox Body GT** | 🏆 **Scenario 3: Coin Collector** (Collect 100 Coins) |
| **4** | **Gen 4 (2003)** | **2003 SVT Cobra Mystichrome** | 🏆 **Scenario 4: Velocity Master** (Reach 180 KM/H Speed) |
| **5** | **Gen 5 (2013)** | **2013 Shelby GT500** | 🏆 **Scenario 5: Endurance Legend** (Reach Level 4) |
| **6** | **Gen 6 (2024)** | **2024 Mustang Dark Horse** | 🏆 **Scenario 6: Game Completion Master** (100% Completion) |

---

## 🏗️ Project Architecture

```
FuelRushMaui/
├── Models/              # Vehicle, Obstacle, Collectible, HighScore models
├── Renderers/           # GameCanvasDrawable (60 FPS canvas graphics & 6-car renderer)
├── Services/            # GameEngine, GarageService, SoundService (WinMM & Tokyo Drift BGM), StorageService
├── ViewModels/          # GarageViewModel & binding context
├── Views/               # LoadingPage, GarageModalView, HighScoresModalView, MainPage
├── Resources/
│   ├── Images/          # Mustang car images, clean loader car, steering wheel
│   └── Raw/             # Tokyo Drift background music (tokyo_drift_bgm.wav)
└── FuelRushMaui.csproj  # .NET 10.0 MAUI configuration
```

---

## 💻 Local Build & Compilation

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or VS Code with .NET MAUI Workload

### Commands

```bash
# Clone the repository
git clone https://github.com/praveshbalaji/FuelRushMaui.git

# Navigate to project folder
cd FuelRushMaui

# Build for Windows
dotnet build -f net10.0-windows10.0.19041.0

# Build Android APK directly
dotnet publish -f net10.0-android -c Release -p:AndroidKeyStore=false
```

---

## 🤝 License & Contact

Designed and developed with passion by **Balaji ([@praveshbalaji](https://github.com/praveshbalaji))**. Distributed under the **MIT License**.
