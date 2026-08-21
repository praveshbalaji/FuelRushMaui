<div align="center">
  <img src="Resources/Images/game_icon.png" width="140" alt="Fuel Rush Icon" />
  <h1 style="font-family: 'Montserrat', sans-serif; color: #113979; font-size: 38px; margin-top: 10px;">Fuel Rush</h1>
  <p style="font-family: 'Montserrat', sans-serif; color: #475569; font-size: 18px;"><b>AAA .NET MAUI 3D Racing Simulator &amp; Performance Showcase</b></p>

  <p>
    <a href="https://dotnet.microsoft.com/en-us/apps/maui"><img src="https://img.shields.io/badge/.NET%20MAUI-net10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET MAUI" /></a>
    <a href="https://github.com/praveshbalaji/FuelRushMaui/releases"><img src="https://img.shields.io/badge/Platform-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20macOS-113979?style=for-the-badge" alt="Platforms" /></a>
    <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge" alt="License" /></a>
  </p>
</div>

---

<div align="center" style="background: linear-gradient(135deg, #0a0e17 0%, #113979 100%); padding: 30px; border-radius: 16px; margin: 25px 0;">
  <h2 style="color: #FFD700; font-family: 'Montserrat', sans-serif; margin-top: 0; font-size: 26px;">📲 Download Game APK (`fuelrush.apk`)</h2>
  <p style="color: #E2E8F0; font-size: 16px; max-width: 600px; margin: 0 auto 20px auto;">
    Play <b>Fuel Rush</b> directly on your Android phone! Download the pre-built signed APK file below:
  </p>
  
  <p>
    <a href="https://github.com/praveshbalaji/FuelRushMaui/releases/latest/download/fuelrush.apk" style="background-color: #22C55E; color: #FFFFFF; font-family: 'Montserrat', sans-serif; font-weight: bold; font-size: 20px; padding: 16px 36px; text-decoration: none; border-radius: 12px; display: inline-block; box-shadow: 0 4px 14px rgba(34, 197, 94, 0.4);">
      ⚡ DOWNLOAD FUELRUSH.APK (LATEST)
    </a>
  </p>
  <p style="color: #94A3B8; font-size: 14px; margin-top: 10px;">
    Alternative: <a href="https://github.com/praveshbalaji/FuelRushMaui/releases" style="color: #38BDF8;">View All GitHub Releases &amp; Assets</a>
  </p>
</div>

### 📲 Quick Installation Instructions for Android Mobile

1. **Download the APK:** Tap the **`DOWNLOAD FUELRUSH.APK`** button above directly on your phone.
2. **Allow Installation from Unknown Sources:**
   - If prompted by your browser (Chrome/Files), tap **Settings** and toggle **Allow from this source** to **ON**.
   - Or go to **Settings > Apps > Special App Access > Install Unknown Apps** on your phone.
3. **Bypass Play Protect Warning (Self-Signed APK):**
   - Because this app is independently published, Google Play Protect may show a warning.
   - Tap **More details** and then select **Install anyway**.
4. **Already Have Previous Build Installed?**
   - Uninstall any older version of `FuelRushMaui` or `Fuel Rush` from your phone first to avoid update signature conflicts.

---

<h2 style="font-family: 'Montserrat', sans-serif; color: #113979;">🌟 Project Overview &amp; Architecture</h2>

**Fuel Rush** is an arcade 3D racing simulator designed and engineered from scratch in **.NET MAUI (C# 10)**. It serves as an architectural showcase demonstrating real-time 60 FPS canvas graphics, continuous vector physics, interactive touch gesture steering controls, multi-tier state management, and multiplatform delivery.

### ⚡ Architectural Highlights
- <b style="color: #113979;">Custom 60 FPS Canvas Engine</b>: Built entirely on `Microsoft.Maui.Graphics` with real-time vector highway rendering, dynamic particle systems (nitro exhaust flames, road sparks, smoke), off-road terrain parallax, and camera shake physics.
- <b style="color: #113979;">Tactile Steering Wheel Controls</b>: Real-time 3D Ford Mustang steering wheel featuring 90° center-axis rotation, normalized touch-drag gesture mapping (`-1.0` to `+1.0`), and cubic spring return-to-center physics.
- <b style="color: #113979;">Mustang Garage &amp; Scenario Achievement System</b>: Features 6 generations of Ford Mustang vehicles (1965 Fastback to 2024 Dark Horse) unlocked dynamically via fixed scenario completion metrics (level gas stations, top speed thresholds, coin milestones).
- <b style="color: #113979;">AI-Accelerated Engineering</b>: Engineered in **under 1 week** utilizing generative AI pair programming for real-time asset processing, mathematical gesture curve optimization, and cross-platform compilation.

---

<h2 style="font-family: 'Montserrat', sans-serif; color: #113979;">🏎️ Mustang Garage Lineup</h2>

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
| **6** | **Gen 6 (2024)** | **2024 Mustang Dark Horse** | 🏆 **Scenario 6: Game Completion Master** (100% Master) |

---

<h2 style="font-family: 'Montserrat', sans-serif; color: #113979;">💻 Local Build &amp; Compilation</h2>

### Prerequisites
- [.NET 10.0 Preview SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 / VS Code with .NET MAUI Workload

```bash
# Clone the repository
git clone https://github.com/praveshbalaji/FuelRushMaui.git

# Navigate to project folder
cd FuelRushMaui

# Build Android APK directly
dotnet publish -f net10.0-android -c Release -p:AndroidKeyStore=false

# Output APK location:
# bin/Release/net10.0-android/publish/fuelrush.apk
```

---

<h2 style="font-family: 'Montserrat', sans-serif; color: #113979;">🤝 License &amp; Contact</h2>

Designed and developed with passion by **Balaji ([@praveshbalaji](https://github.com/praveshbalaji))**. Distributed under the **MIT License**.
