# 🏎️ FuelRushMaui — AAA .NET MAUI 3D Racing Simulator

[![Framework](https://img.shields.io/badge/.NET%20MAUI-net10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/en-us/apps/maui)
[![Language](https://img.shields.io/badge/C%23-10.0-239120?style=for-the-badge&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Platforms](https://img.shields.io/badge/Platforms-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20macOS-00E5FF?style=for-the-badge)](https://dotnet.microsoft.com/en-us/apps/maui)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

An arcade 3D racing simulator designed, architected, and engineered in **.NET MAUI** featuring custom canvas graphics rendering, 60 FPS real-time physics, interactive steering wheel gesture controls, dynamic particle systems, and 6 generations of iconic Ford Mustang vehicles linked to scenario achievement unlocks.

---

## 🌟 Creator & Project Highlights

> **Designed, Architected & Engineered by Balaji ([@praveshbalaji](https://github.com/praveshbalaji))**
> 
> *Showcasing advanced cross-platform .NET MAUI software engineering, complex game physics calculations, custom canvas drawing, and AI-accelerated rapid development—concept to production-grade deployment delivered in **under 1 week**.*

### ⚡ Rapid AI-Accelerated Engineering
This project stands as a benchmark for modern AI-assisted software architecture. By leveraging generative AI pair programming for real-time image asset processing, mathematical gesture physics, and cross-platform XAML/C# optimization, full multiplatform delivery was achieved seamlessly across **Android, iOS, Windows, and macOS**.

---

## 🚀 Key Architectural & Game Features

### 1. 🏎️ Custom 60 FPS Graphics & Canvas Game Engine
- **Direct Canvas Rendering**: Engineered on `Microsoft.Maui.Graphics` with `IDrawable` and `GraphicsView` for high-performance 60 FPS rendering.
- **Dynamic Environment**: Features a 5-lane scrolling highway, off-road terrain, night sky parallax background, glowing curb blocks, and street light poles.
- **Visual Effects**: Nitro speed blur streak lines, camera shake recoil on boost/collisions, and real-time particle engine for sparks, smoke, and exhaust nitro flames.

### 2. 🛞 Real-Time Steering Wheel Physics & Controls
- **Tactile Gesture Rotation**: Interactive 3D Ford Mustang steering wheel with center-axis rotation (-90° to +90°).
- **Smooth Analog Easing**: Built with continuous normalized float mapping (`-1.0` to `+1.0`) and cubic spring return-to-center physics (`AnimateSteeringReturnToCenter`) to prevent snap rotation.
- **Simulator HUD Pedals**: Ribbed brake pedal with decelerator physics and tall chrome accelerator pedal with progressive RPM revving.

### 3. 🏆 Mustang Garage & Scenario Achievement System
Includes 6 generations of Ford Mustang vehicles with custom performance specifications (Top Speed, Acceleration, Fuel Efficiency, Handling):

| # | Generation | Mustang Model | Scenario Unlock Achievement Milestone |
|---|---|---|---|
| **1** | **Gen 1 (1965)** | **1965 Mustang Fastback GT** | 🏆 **Scenario 1: Vintage Pioneer** (Default Starter Car) |
| **2** | **Gen 2 (1974)** | **1974 Mustang II Coupe** | 🏆 **Scenario 2: Gas Station Pioneer** (Reach Gas Station #1) |
| **3** | **Gen 3 (1990)** | **1990 Fox Body GT** | 🏆 **Scenario 3: Coin Collector** (Accumulate 100 Coins) |
| **4** | **Gen 4 (2003)** | **2003 SVT Cobra Mystichrome** | 🏆 **Scenario 4: Velocity Master** (Reach 180 KM/H Speed) |
| **5** | **Gen 5 (2013)** | **2013 Shelby GT500** | 🏆 **Scenario 5: Endurance Legend** (Reach Gas Station #4) |
| **6** | **Gen 6 (2024)** | **2024 Mustang Dark Horse** | 🏆 **Scenario 6: Game Completion Master** (100% Game Completion) |

### 4. ⚡ Turbo Car Motion Loader & Custom App Icon
- Custom high-impact app logo icon featuring neon green "FUEL RUSH" typography and carbon fiber running horse badge.
- Motion loader screen featuring a driving Mustang with neon green turbo flame boost trails across a speed track with real-time percentage progress.

---

## 🛠️ Technology Stack

- **Framework**: .NET MAUI (.NET 10.0)
- **Language**: C# 10.0 / XAML
- **Graphics & Physics**: `Microsoft.Maui.Graphics`, Custom Vector Physics Engine, Math Easing Curves
- **State & Data**: Local persistent storage service, High scores, Preference JSON serialization
- **Target Platforms**: Android (API 21+), iOS (15+), macOS (MacCatalyst), Windows (WinUI 3)

---

## 💻 Getting Started & Running Locally

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 / VS Code with .NET MAUI workload installed

### Clone & Run
```bash
# Clone the repository
git clone https://github.com/praveshbalaji/FuelRushMaui.git

# Navigate to project folder
cd FuelRushMaui

# Restore dependencies & build for Windows
dotnet build -c Debug -f net10.0-windows10.0.19041.0

# Run project
dotnet run -f net10.0-windows10.0.19041.0
```

---

## 📄 License
Distributed under the **MIT License**. See `LICENSE` for details.

---

## 🤝 Connect & Contact
Built with passion by **Balaji**.
- **GitHub**: [@praveshbalaji](https://github.com/praveshbalaji)
- **LinkedIn**: [Connect on LinkedIn](https://www.linkedin.com/in/praveshbalaji/)
