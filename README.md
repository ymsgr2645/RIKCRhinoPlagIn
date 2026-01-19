# LayerTabs

Tabbed layer navigation plugin for Rhino

## Features

- Auto tabs from top-level layers
- Manual group creation
- Favorites tab
- Search/filter layers
- Tab position: Top or Side

## Install

1. Download from Releases
2. Drag .rhp file into Rhino
3. Restart Rhino

## Commands

- `LayerTabs` - Toggle panel
- `LayerTabsSettings` - Open settings
- `LayerTabsAddGroup` - Create manual group
- `LayerTabsFavorite` - Add/remove favorite

## Usage

- Double-click layer to set current
- Click tab to filter layers
- Use search box to find layers

## Build

```bash
dotnet build src/LayerTabs.csproj -c Release -f net48  # Rhino 7
dotnet build src/LayerTabs.csproj -c Release -f net7.0 # Rhino 8
```

## Author

Concept & Logic: RIKUSATO
