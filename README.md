# PornPaster

A Windows utility for quickly pasting random images from configured folders using hotkeys.

## Features

- Configure multiple hotkeys for different image folders
- Choose between random or round-robin folder selection
- Search subdirectories option
- Optional auto-paste functionality
- System tray application for easy access
- Configuration saved per user

## Installation

### Option 1: Use the Installer
1. Go to the [Releases](https://github.com/YourUsername/PornPaster/releases) page
2. Download the latest `PornPaster_Setup.exe`
3. Run the installer and follow the on-screen instructions to complete the installation

### Option 2: Download Standalone Executable (Legacy)
1. Go to the [Releases](https://github.com/YourUsername/PornPaster/releases) page
2. Download the latest `PornPaster.exe`
3. Run the executable - no installation needed

### Option 3: Build from Source
1. Clone the repository
2. Make sure you have .NET 6.0 SDK installed
3. Run `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true`
4. Find the executable in `bin/Release/net6.0-windows/win-x64/publish/PornPaster.exe`

## Usage

1. Run the application
2. Right-click the tray icon and select "Configure"
3. Add hotkeys and configure their settings:
   - Name: A descriptive name for the hotkey
   - Key: The keyboard key to trigger image pasting
   - Folders: One or more folders containing images
   - Search Subdirectories: Whether to include images in subfolders
   - Auto Paste: Whether to automatically paste after copying
   - Folder Selection: Choose between Random or Round-Robin mode
4. Press Save to apply changes
5. Use configured hotkeys to paste random images

## Configuration

Settings are stored in: `%APPDATA%\PornPaster\config.json`

## Building

To build a standalone executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## Requirements

- Windows 10 or later
- .NET 6.0 Runtime (included in standalone build) 