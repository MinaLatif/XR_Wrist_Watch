# XR Wrist Watch - Development TODO

## Project Vision
Transform this wrist watch app into a complete time management system for Meta Quest, helping users stay aware of time and maintain healthy VR habits.

---

## Phase 1: Core Watch (Current) ✅

- [x] Basic wrist tracking using XR Hands
- [x] Digital time display (12-hour format with AM/PM)
- [x] Date display
- [x] AR passthrough mode support
- [x] VR mode with palm-up gesture
- [x] Tracking loss handling with fallback UI
- [x] Smooth tracking with jitter reduction
- [x] One-click scene setup in Editor
- [x] README documentation

---

## Phase 2: Watch Customization

### Watch Faces
- [ ] Analog clock face option
- [ ] Multiple digital styles (minimal, detailed, retro)
- [ ] Color themes (dark, light, custom colors)
- [ ] Watch face selector UI

### Display Options
- [ ] 24-hour time format toggle
- [ ] Show/hide seconds
- [ ] Show/hide date
- [ ] Different date formats (US, EU, ISO)
- [ ] Battery level indicator (Quest battery)

### Visual Polish
- [ ] Rounded corners on watch background
- [ ] Watch bezel/frame design
- [ ] Glow effect option
- [ ] Shadow for depth

---

## Phase 3: Alarms & Notifications

### Alarm System
- [ ] Set multiple alarms
- [ ] Alarm sound/vibration
- [ ] Snooze functionality
- [ ] Recurring alarms (daily, weekdays, weekends)
- [ ] Alarm labels/names
- [ ] Visual alarm notification in VR

### Quick Timers
- [ ] Countdown timer
- [ ] Stopwatch
- [ ] Pomodoro timer (25min work / 5min break)
- [ ] Timer presets (5, 10, 15, 30 min)

### Notifications
- [ ] Gentle notification system (non-intrusive)
- [ ] Notification history
- [ ] Do Not Disturb mode

---

## Phase 4: Play Time Management

### Session Tracking
- [ ] Track current session duration
- [ ] Display "Playing for: X hours Y minutes"
- [ ] Daily/weekly play time statistics
- [ ] Play time history log

### Break Reminders
- [ ] Configurable break intervals (30min, 1hr, 2hr)
- [ ] Break reminder notifications
- [ ] Suggested break activities
- [ ] "Take a break" fullscreen reminder option
- [ ] Parental controls / time limits

### Health Features
- [ ] Eye strain reminders (look away from screen)
- [ ] Stand up / stretch reminders
- [ ] Hydration reminders
- [ ] Customizable reminder messages

---

## Phase 5: Settings & Persistence

### Settings UI
- [ ] In-VR settings panel
- [ ] Watch position adjustment UI
- [ ] All preferences configurable in VR
- [ ] Settings accessible via watch tap/gesture

### Data Persistence
- [ ] Save user preferences
- [ ] Save alarm configurations
- [ ] Save play time statistics
- [ ] Cloud sync (optional)

### Accessibility
- [ ] Adjustable text size
- [ ] High contrast mode
- [ ] Left hand / right hand option
- [ ] Colorblind-friendly themes

---

## Phase 6: Advanced Features

### Calendar Integration
- [ ] Show upcoming events
- [ ] Event reminders
- [ ] Google Calendar sync (if possible)
- [ ] Quick event creation

### World Clock
- [ ] Multiple time zones
- [ ] Quick timezone switcher
- [ ] Time zone converter

### Widgets
- [ ] Weather display (if API available)
- [ ] Quick notes
- [ ] App shortcuts

---

## Phase 7: Polish & Release

### Performance
- [ ] Profile and optimize for Quest
- [ ] Reduce draw calls
- [ ] Optimize hand tracking polling
- [ ] Battery usage optimization

### Testing
- [ ] Test on Quest 2
- [ ] Test on Quest 3
- [ ] Test on Quest Pro
- [ ] Edge case testing (tracking loss, low battery, etc.)

### Release Preparation
- [ ] App icon and branding
- [ ] Screenshots and store assets
- [ ] Privacy policy
- [ ] App Lab / Store submission

---

## Bug Fixes & Issues

- [ ] (Add bugs here as discovered during testing)

---

## Ideas / Backlog

- [ ] Voice commands ("Hey watch, set timer for 10 minutes")
- [ ] Gesture controls (tap watch to switch modes)
- [ ] Integration with other VR apps
- [ ] Multiplayer sync (show friends' availability)
- [ ] Sleep mode (dim watch in dark environments)
- [ ] Auto-brightness based on environment
- [ ] Haptic feedback on Quest Pro

---

## Technical Debt

- [ ] Add unit tests for time formatting
- [ ] Add XML documentation to all public methods
- [ ] Create prefab variants for different watch styles
- [ ] Refactor large scripts into smaller components

---

## Notes

### Priority Order
1. Get basic watch working reliably (Phase 1) ✅
2. Add alarms and timers (Phase 3) - Most requested feature
3. Play time tracking (Phase 4) - Core value proposition
4. Watch customization (Phase 2) - Nice to have
5. Settings persistence (Phase 5) - Required for usability
6. Advanced features (Phase 6) - Future expansion

### Testing Checklist (Run Before Each Build)
- [ ] Watch appears on wrist correctly
- [ ] Time is accurate
- [ ] Palm-up gesture works (VR mode)
- [ ] Tracking loss message appears/disappears correctly
- [ ] No jitter or lag in tracking
- [ ] App launches quickly
- [ ] No crashes after 10+ minutes of use

---

*Last updated: January 2026*
