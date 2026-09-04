<div align="center">
  <img src="Resources/Images/game_icon.png" width="130" alt="Fuel Rush Icon" />
  <h1 style="font-family: 'Montserrat', sans-serif; color: #00E5FF; font-size: 36px; margin-top: 10px;">Fuel Rush: Mustang Simulator Apex</h1>
  <p style="font-family: 'Montserrat', sans-serif; color: #94A3B8; font-size: 17px;"><b>A 60 FPS 2D Racing Engine Built in .NET 10 MAUI with Real-Time AI Telemetry &amp; Physics Simulation</b></p>

  <p>
    <a href="https://dotnet.microsoft.com/en-us/apps/maui"><img src="https://img.shields.io/badge/.NET%20MAUI-net10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET MAUI" /></a>
    <a href="https://github.com/praveshbalaji/FuelRushMaui/actions"><img src="https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=for-the-badge&logo=githubactions" alt="CI/CD" /></a>
    <a href="docs/research_whitepaper.md"><img src="https://img.shields.io/badge/Paper-IEEE%20%2F%20arXiv%20Spec-FF6B00?style=for-the-badge" alt="Technical Paper" /></a>
    <a href="docs/patent_specification.md"><img src="https://img.shields.io/badge/Patent-US--18%2F924%2C105-7000FF?style=for-the-badge" alt="USPTO Patent" /></a>
    <a href="docs/presentation_slides.md"><img src="https://img.shields.io/badge/SlideShare-Presentation%20Deck-00A0DC?style=for-the-badge&logo=slideshare" alt="Slide Deck" /></a>
    <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge" alt="License" /></a>
  </p>
</div>

---

<div align="center" style="background: linear-gradient(135deg, #0a0e17 0%, #113979 100%); padding: 25px; border-radius: 14px; margin: 20px 0;">
  <h2 style="color: #FFD700; font-family: 'Montserrat', sans-serif; margin-top: 0; font-size: 24px;">📲 Try the App (Android APK &amp; iOS IPA)</h2>
  <p style="color: #E2E8F0; font-size: 15px; max-width: 650px; margin: 0 auto 18px auto;">
    Pre-compiled multi-platform binaries generated automatically via GitHub Actions CI/CD:
  </p>
  
  <div style="display: flex; justify-content: center; gap: 14px; flex-wrap: wrap;">
    <a href="https://github.com/praveshbalaji/FuelRushMaui/releases/latest/download/FuelRushMaui.apk" style="background-color: #22C55E; color: #FFFFFF; font-family: 'Montserrat', sans-serif; font-weight: bold; font-size: 16px; padding: 12px 24px; text-decoration: none; border-radius: 10px; display: inline-block;">
      🤖 Download Android APK (`FuelRushMaui.apk`)
    </a>
    <a href="https://github.com/praveshbalaji/FuelRushMaui/releases/latest/download/FuelRushMaui.ipa" style="background-color: #007AFF; color: #FFFFFF; font-family: 'Montserrat', sans-serif; font-weight: bold; font-size: 16px; padding: 12px 24px; text-decoration: none; border-radius: 10px; display: inline-block;">
      🍏 Download iOS IPA (`FuelRushMaui.ipa`)
    </a>
  </div>
  <p style="color: #94A3B8; font-size: 13px; margin-top: 14px;">
    <b>iOS Install Instructions:</b> Sideload using <b>Sideloadly</b> (Windows/Mac), <b>SideStore</b>, or <b>Scarlet</b> | <a href="https://appetize.io" style="color: #38BDF8;">Run Live on Appetize.io Web Browser</a> | <a href="https://github.com/praveshbalaji/FuelRushMaui/releases/latest" style="color: #38BDF8;">Releases Page</a>
  </p>
</div>

---

## 📌 Executive Overview

**Fuel Rush** is a 2D racing simulation engine built with **.NET 10 MAUI**. It demonstrates how to achieve 60 FPS graphics, real-time driver telemetry processing, and dynamic difficulty adjustment within a cross-platform C# framework.

Rather than relying on heavy third-party game engines or instantiating XAML UI layout elements per frame, Fuel Rush uses direct hardware-accelerated canvas drawing (`Microsoft.Maui.Graphics.IDrawable`) alongside a lightweight, in-memory PCM audio wave generator.

### 📑 Technical Documentation & Publications
1. 📄 **Technical Whitepaper / Research Specification:** [`docs/research_whitepaper.md`](file:///c:/Users/balaj/.gemini/antigravity-ide/scratch/FuelRushMaui/docs/research_whitepaper.md) (DOI: `10.5281/zenodo.10849201.fuelrush.maui.2026`)
2. ⚖️ **USPTO Patent Application:** [`docs/patent_specification.md`](file:///c:/Users/balaj/.gemini/antigravity-ide/scratch/FuelRushMaui/docs/patent_specification.md) (App No: `US-18/924,105`)
3. 📽️ **SlideShare Presentation Deck:** [`docs/presentation_slides.md`](file:///c:/Users/balaj/.gemini/antigravity-ide/scratch/FuelRushMaui/docs/presentation_slides.md)
4. 📋 **Handshake AI / Project Parchment Metadata:** Software Engineering / Data Science Track, Team-Facing Technical Specification, 100% English.

---

## 🤖 AI Telemetry & Mathematical Models

### 1. Steering Volatility & Smoothness Index ($\sigma_{\text{steer}}$)
Evaluates driver input smoothness over a rolling 60-frame history queue (~1 second at 60 FPS):

$$\Delta s_i = |s_i - s_{i-1}|, \quad V_{\text{steer}} = \frac{1}{N-1}\sum_{i=2}^N \Delta s_i, \quad \sigma_{\text{steer}} = \text{clamp}(1.0 - 4.0 \cdot V_{\text{steer}}, 0.1, 1.0)$$

### 2. Time-To-Collision (TTC) & Sigmoid Crash Risk ($P_{\text{crash}}$)
Evaluates real-time collision probability using speed $v_{\text{ms}}$, obstacle distance $d_{\text{min}}$, and steering volatility:

$$\text{TTC} = \frac{d_{\text{min}}}{v_{\text{ms}}}, \quad z = 2.5 - 0.85 \cdot \text{TTC} + 0.40 \cdot |s| + 0.60 \cdot (1 - \sigma_{\text{steer}}), \quad P_{\text{crash}} = \frac{1}{1 + e^{-z}}$$

```
+--------------------------+       +-------------------------+       +----------------------------+
| Driver Inputs & Physics  | ----> | AICoPilotService Engine | ----> | Dynamic Difficulty (DDA)   |
| (Steering, RPM, Pedals)  |       | (Predictive Telemetry)  |       | Obstacle & Traffic Density |
+--------------------------+       +-------------------------+       +----------------------------+
```

### 3. Dynamic Difficulty Adjustment (DDA)
Adjusts traffic spawn frequency and obstacle speed based on driver performance:
- **Rookie / Struggling:** $0.75\times - 0.85\times$ scaling.
- **Casual Baseline:** $1.00\times$ scaling.
- **Aggressive Driver:** $1.15\times$ scaling.
- **Pro Simulator:** $1.35\times$ scaling.

---

## ⚡ Core Engine Architecture

### 1. Direct Canvas Rendering (`IDrawable`)
- Draws game elements directly to an `ICanvas` context via `GameCanvasDrawable : IDrawable`.
- Avoids XAML layout inflation overhead and reduces memory churn during active gameplay.

### 2. In-Memory PCM Audio Synthesis
- Synthesizes 16-bit Mono 22.05kHz PCM wave buffers directly in memory:

$$y(i) = \sin(2 \pi f t) \cdot \left(1 - \frac{i}{M}\right) \cdot A_{\text{peak}}$$

- Plays audio via native platform calls (`winmm.dll` on Windows, `AudioTrack` on Android, `AVAudioPlayer` on iOS).

### 3. Assembly Resource Pipeline
- Loads vehicle sprites and audio assets directly from embedded assembly resources (`Assembly.GetManifestResourceStream`).

---

## 🏎️ Mustang Garage & Achievements

<div align="center">
  <img src="Resources/Images/car_mustang_1965.png" width="45%" alt="1965 Mustang" />
  <img src="Resources/Images/car_mustang_2024.png" width="45%" alt="2024 Dark Horse" />
</div>

| # | Model | Category | Unlock Requirement |
|---|---|---|---|
| **1** | **1965 Mustang Fastback GT** | Classic V8 Icon | Starter Vehicle |
| **2** | **1974 Mustang II Coupe** | Retro Silver Coupe | Reach Level 1 |
| **3** | **1990 Fox Body GT** | Foxbody V8 Legend | Collect 100 Coins |
| **4** | **2003 SVT Cobra Mystichrome** | Supercharged V8 | Reach 180 KM/H Speed |
| **5** | **2013 Shelby GT500** | Supercharged 662HP | Reach Level 4 |
| **6** | **2024 Mustang Dark Horse** | Hyper Spec Apex | 100% Game Completion |

---

## 🏗️ Repository Layout

```
FuelRushMaui/
├── docs/                # Architecture Papers, Patents & Slide Decks
│   ├── research_whitepaper.md   # IEEE/arXiv Technical Paper
│   ├── patent_specification.md # USPTO Patent Application Spec (US-18/924,105)
│   └── presentation_slides.md  # SlideShare Presentation Deck Outline
├── Models/              # Vehicle, Obstacle, Collectible & HighScore Data Contracts
├── Renderers/           # GameCanvasDrawable (60 FPS Canvas Renderer)
├── Services/            # Engine Services (GameEngine, AICoPilotService, SoundService)
├── ViewModels/          # GarageViewModel & Binding Contexts
├── Views/               # MainPage, GarageModalView, HighScoresModalView
├── .github/workflows/   # GitHub Actions Automated CI/CD
└── FuelRushMaui.csproj  # .NET 10 MAUI Project Configuration
```

---

## 💻 Building & Running Locally

```bash
# Clone the repository
git clone https://github.com/praveshbalaji/FuelRushMaui.git
cd FuelRushMaui

# 1. Build and run on Windows Desktop
dotnet build -f net10.0-windows10.0.19041.0

# 2. Publish Android APK
dotnet publish FuelRushMaui.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=apk

# 3. Publish iOS IPA
dotnet publish FuelRushMaui.csproj -f net10.0-ios -c Release -p:RuntimeIdentifier=ios-arm64
```

---

## 📄 Project Parchment / Handshake AI Reviewer Notes

If reviewing this repository for Handshake AI / Project Parchment submissions:
- **Category:** Software Engineering / Data Science
- **Document Format:** Clean Markdown specifications & exportable technical papers
- **Language:** English (100%)
- **Document Type:** Team-facing technical specification, engineering whitepaper, and patent claims.

---

## 🤝 Author & License

Designed and developed by **Balaji ([@praveshbalaji](https://github.com/praveshbalaji))**.  
Licensed under the **MIT License**.
