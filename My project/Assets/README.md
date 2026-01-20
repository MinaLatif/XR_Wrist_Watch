# XR Wrist Watch

A Meta Quest wrist watch app that displays the current time on your wrist using hand tracking. Works in both AR passthrough mode and VR mode.

## Purpose

**The Problem:** It's easy to lose track of time while immersed in VR. Taking off your headset to check the time breaks immersion, and there's no native always-visible clock on Quest.

**The Solution:** A virtual wristwatch that appears on your left wrist, accessible anytime with a natural gesture - just like checking a real watch.

**Future Vision:** This project aims to become an all-purpose clock and alarm system for Meta Quest, helping users:
- Manage play time and take healthy breaks
- Set alarms and reminders without leaving VR
- Stay aware of real-world time commitments
- Maintain a healthy balance between VR and reality

## Features

### Current Features
- **Wrist-mounted time display** - Digital clock attached to your left wrist
- **AR Passthrough mode** - See the real world with the watch overlay
- **VR mode with palm gesture** - Watch appears only when you twist your wrist to look at it (like a real watch)
- **Hand tracking** - No controllers needed, uses Quest's built-in hand tracking
- **Smooth tracking** - Watch follows your wrist movement with optional smoothing to reduce jitter
- **Tracking loss handling** - Shows a helpful message when hand goes out of view
- **12-hour format with date** - Displays time (e.g., "10:30 AM") and date (e.g., "Mon, Jan 19")

### Planned Features
- [ ] Alarm system with VR notifications
- [ ] Play time tracker and session duration display
- [ ] Break reminders (e.g., "You've been playing for 1 hour")
- [ ] Customizable watch faces
- [ ] Stopwatch and timer functions
- [ ] Calendar integration
- [ ] Multiple time zones
- [ ] Analog watch face option

## Requirements

- **Unity 6000.2** or later
- **Meta Quest 2/3/Pro** headset
- **Packages** (already included):
  - XR Hands 1.6.1
  - OpenXR 1.15.1
  - Meta OpenXR 2.3.0
  - XR Interaction Toolkit 3.2.1
  - TextMeshPro

## Quick Start

### One-Click Setup

1. Open the project in Unity
2. Wait for scripts to compile
3. Go to `GameObject > XR > Complete Wrist Watch Scene Setup`
4. Enable passthrough (see below)
5. Build and deploy to Quest

### Enable Passthrough (Required for AR Mode)

```
Edit > Project Settings > XR Plug-in Management > OpenXR > Android tab
Enable: "Meta Quest: Camera (Passthrough)"
```

## Modes

### AR Passthrough Mode (Default)
- See the real world through Quest cameras
- Watch is always visible on your wrist
- Great for quick time checks without removing headset

### VR Mode
- For use inside VR games/experiences
- Enable `Require Palm Up` in the WristTracker component
- Watch only appears when you twist your wrist to look at it
- Mimics the natural gesture of checking a real watch

## Project Structure

```
Assets/
├── Scripts/
│   ├── WristTracker.cs           # Tracks wrist joint, positions watch
│   ├── TimeDisplay.cs            # Updates time and date text
│   ├── TrackingLostHandler.cs    # Handles tracking loss UI
│   └── Editor/
│       └── WristWatchSetup.cs    # One-click scene setup
├── Scenes/
│   └── SampleScene.unity         # Main scene
└── Settings/
    └── (URP render settings)
```

## Components

### WristTracker
Tracks the left wrist using XR Hands and positions the watch UI.

| Setting | Description |
|---------|-------------|
| Position Offset | Offset from wrist joint (default: 2cm up) |
| Rotation Offset | Rotation to face watch upward |
| Require Palm Up | Only show watch when palm faces up (VR mode) |
| Palm Up Threshold | Angle tolerance for palm detection (default: 45°) |
| Enable Smoothing | Smooth tracking to reduce jitter |

### TimeDisplay
Updates the time and date display.

| Setting | Description |
|---------|-------------|
| Time Format | Format string (default: "h:mm tt" = 10:30 AM) |
| Date Format | Format string (default: "ddd, MMM d" = Mon, Jan 19) |
| Update Interval | How often to refresh (default: 0.5s) |

### TrackingLostHandler
Manages UI visibility when hand tracking is lost.

| Setting | Description |
|---------|-------------|
| Fade Duration | Time to fade in/out (default: 0.3s) |
| Debounce Time | Prevents flickering (default: 0.15s) |

## Customization

### Change Time Format
In the TimeDisplay component:
- `"h:mm tt"` → 1:30 PM (12-hour)
- `"HH:mm"` → 13:30 (24-hour)
- `"h:mm:ss tt"` → 1:30:45 PM (with seconds)

### Change Date Format
- `"ddd, MMM d"` → Mon, Jan 19
- `"dddd, MMMM d"` → Monday, January 19
- `"MM/dd/yyyy"` → 01/19/2026

### Adjust Watch Position
Modify `Position Offset` in WristTracker:
- Increase Y to move watch further from wrist (prevents clipping)
- Default: (0, 0.02, 0) = 2cm above wrist

### Change Watch Size
Select WatchCanvas and modify the scale:
- Default: (0.001, 0.001, 0.001)
- Larger values = bigger watch

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Watch not appearing | Check XRHandTrackingEvents is assigned to WristTracker |
| Black screen (no passthrough) | Enable "Meta Quest: Camera (Passthrough)" in OpenXR settings |
| Jittery tracking | Increase Smooth Speed or enable smoothing |
| Watch clips through arm | Increase Position Offset Y value |
| Hand tracking not working | Enable hand tracking in Quest system settings |

## Building for Quest

1. `File > Build Settings`
2. Platform: Android
3. Texture Compression: ASTC
4. Connect Quest via USB or use Air Link
5. `Build and Run`

## Contributing

This is an early-stage project. Future contributions welcome for:
- Alarm system implementation
- Additional watch face designs
- Play time tracking features
- Settings/preferences UI

## License

[Your license here]

## Acknowledgments

- Built with Unity XR Hands and Meta OpenXR
- Inspired by the need to stay aware of time while in VR
