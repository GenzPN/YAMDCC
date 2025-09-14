# Auto Refresh Rate Feature

YAMDCC now supports automatic screen refresh rate changes based on power state (AC power vs battery).

## Features

- **Automatic switching**: Configure different refresh rates for AC power and battery mode
- **Power state detection**: Automatically detects when you plug/unplug your laptop
- **Manual control**: Set refresh rates manually via CLI commands
- **Configuration persistence**: Settings are saved and restored across reboots

## CLI Usage

### Set a specific refresh rate
```
yamdcc.exe -refreshrate 144
```

### Enable/toggle automatic refresh rate switching
```
yamdcc.exe -autorefreshrate
```

### Configure AC and battery refresh rates
You can configure the automatic refresh rates by editing your YAMDCC config file (CurrentConfig.xml) or using the config editor.

Add these properties to your config:
```xml
<AutoRefreshRateEnabled>true</AutoRefreshRateEnabled>
<RefreshRateAC>144</RefreshRateAC>
<RefreshRateBattery>60</RefreshRateBattery>
```

### Example usage
```bash
# Set high refresh rate for gaming on AC power
yamdcc.exe -refreshrate 144

# Enable automatic switching
yamdcc.exe -autorefreshrate

# Apply the configuration
yamdcc.exe -apply
```

## How it works

1. Configure your desired refresh rates for AC and battery modes
2. Enable auto refresh rate in your config
3. YAMDCC service monitors power events (AC plug/unplug)
4. When power state changes, the screen refresh rate is automatically adjusted
5. Manual refresh rate changes are also supported via CLI

## Supported refresh rates

- Range: 30-300 Hz
- Common values: 60 Hz, 120 Hz, 144 Hz, 165 Hz, 240 Hz
- Invalid values are automatically clamped to valid range

## Benefits

- **Better battery life**: Lower refresh rates (60 Hz) save power when on battery
- **Better performance**: Higher refresh rates (144+ Hz) for gaming/work when on AC
- **Automatic**: No manual switching needed - happens automatically on power events