<div align="center">
  <img src="Resources/Images/game_icon.png" width="140" alt="Fuel Rush Maui Icon" />
  <h1 style="font-family: 'Montserrat', sans-serif; color: #113979; font-size: 34px; margin-top: 10px;">FuelRushMaui</h1>
  <p style="font-family: 'Montserrat', sans-serif; color: #475569; font-size: 16px;"><b>AAA .NET MAUI 3D Racing Simulator &amp; Performance Showcase</b></p>

  <p>
    <a href="https://dotnet.microsoft.com/en-us/apps/maui"><img src="https://img.shields.io/badge/.NET%20MAUI-net10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET MAUI" /></a>
    <a href="https://github.com/praveshbalaji/FuelRushMaui/releases"><img src="https://img.shields.io/badge/Platform-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20macOS-113979?style=for-the-badge" alt="Platforms" /></a>
    <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge" alt="License" /></a>
  </p>
</div>

---

<h2 style="font-family: 'Montserrat', sans-serif; color: #113979;">📲 Download &amp; Play</h2>

Experience the simulator directly on your Android device:

<div align="center" style="margin: 20px 0;">
  <a href="https://github.com/praveshbalaji/FuelRushMaui/releases/latest" style="background-color: #113979; color: #FFFFFF; font-family: 'Montserrat', sans-serif; font-weight: bold; font-size: 18px; padding: 14px 28px; text-decoration: none; border-radius: 12px; display: inline-block;">
    📥 DOWNLOAD LATEST ANDROID APK
  </a>
</div>

> **🛡️ Trust & Safety Statement**  
> *Note: Because this app is self-published to showcase my full-stack .NET MAUI skills, Android will ask you to allow installation from 'Unknown Sources'. The APK is cryptographically signed and perfectly safe.*

---

<h2 style="font-family: 'Montserrat', sans-serif; color: #113979;">🌟 Project Overview &amp; Architecture</h2>

**FuelRushMaui** is an arcade 3D racing simulator designed and engineered from scratch in **.NET MAUI (C# 10)**. It serves as an architectural showcase demonstrating real-time 60 FPS canvas graphics, continuous vector physics, interactive touch gesture steering controls, multi-tier state management, and multiplatform delivery.

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

# Restore & build for Windows
dotnet build FuelRushMaui.csproj -f net10.0-windows10.0.19041.0 -c Debug

# Run application
dotnet run -f net10.0-windows10.0.19041.0
```

---

<h2 style="font-family: 'Montserrat', sans-serif; color: #113979;">🤝 License &amp; Contact</h2>

Designed and developed with passion by **Balaji ([@praveshbalaji](https://github.com/praveshbalaji))**. Distributed under the **MIT License**.
