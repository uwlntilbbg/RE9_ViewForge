# RE9_ViewForge

Enhanced FOV controller for **Resident Evil: Requiem**. Keeps the original “separate FOV for TPS/FPS and ADS” behavior and adds profiles, smoothing, hotkeys, and richer UI.

## Features
- Four ready-to-use profiles (Balanced, Action Push, Wide Scout, Director Shot) plus manual selection.
- Auto profile switching based on context (TPS ↔ FPS, ADS-aware).
- Independent FOV values for TPS/FPS and their ADS variants; “hold vanilla FOV” toggle.
- Smooth transitions with configurable strength and smoothing factor.
- Hotkeys for profile cycling, view-mode switching, and vanilla-hold.
- Debug overlay: raw/target/applied FOV, active profile/source, view mode, ADS state.
- UI tweaks: help markers, grouped settings, one-click resets to profile defaults.
- Normalization/validation of settings (clamped mins/maxes, sensible step sizes).

## Prerequisites
- REFramework and the REFramework C# API (csharp-api) — download both from https://github.com/praydog/REFramework-nightly/releases (`RE9.zip` **and** `csharp-api.zip`).
- .NET 10.0 Desktop Runtime x64 — https://dotnet.microsoft.com/en-us/download/dotnet/10.0

## Installation
1. Install prerequisites: extract **both** `RE9.zip` and `csharp-api` into the game folder; install the .NET 10.0 Desktop Runtime x64 if not already installed.
2. Copy `RE9_ViewForge.dll` into `reframework/plugins/managed/` in the game folder.
3. First launch after adding the C# API can take a while—wait until the REFramework console finishes (“setting up script watcher”).
4. In-game, open REFramework UI → `REFramework.NET script generated UI` → `RE9_ViewForge` → adjust FOV/profiles/hotkeys.

## Usage tips
- Tune TPS/FPS/ADS FOV and smoothing; toggle the debug overlay as needed.
- Bind hotkeys for profile cycling, view-mode switching, and vanilla-FOV hold inside the ViewForge panel.
- If something fails to load, confirm REFramework files and dependencies from the same nightly are present next to the game executable.

## Changelog
v1.0.0 — Initial release
