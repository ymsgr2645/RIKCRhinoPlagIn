# LayerTabs - Rhino Layer Panel Extension

## Overview

LayerTabs is a Rhino plugin that adds tabbed navigation to the layer panel, making it easier to navigate large layer hierarchies. The plugin creates a dockable panel that works alongside or replaces the standard layer panel workflow.

---

## Core Features

### 1. Tab Navigation

| Feature | Description |
|---------|-------------|
| Auto Tabs | Top-level layers automatically become tabs |
| Manual Groups | User-defined tab groups (saved per document) |
| Hybrid Mode | Combine auto + manual grouping |
| Tab Position | Top (horizontal) or Side (vertical) - user preference |

### 2. Layer Operations

| Action | Method |
|--------|--------|
| Set Current | Double-click or button |
| Toggle Visibility | Eye icon click |
| Toggle Lock | Lock icon click |
| Expand/Collapse | Arrow click |
| Multi-select | Ctrl/Cmd + click |

### 3. Search & Filter

- Real-time incremental search
- Filter by: name, color, material
- Search scope: current tab or all tabs

### 4. Favorites (★ Tab)

- Pin frequently used layers
- Persists across sessions
- Quick access regardless of hierarchy

---

## Technical Specification

### Platform Support

| Platform | Rhino 7 | Rhino 8 |
|----------|---------|---------|
| Windows | ✓ | ✓ |
| macOS | ✓ | ✓ |

### Architecture

```
LayerTabs/
├── LayerTabsPlugin.cs      # Plugin entry point
├── LayerTabsPanel.cs       # Main dockable panel
├── Core/
│   ├── TabManager.cs       # Tab creation & management
│   ├── LayerSync.cs        # Sync with Rhino layers
│   └── GroupDefinition.cs  # Manual group storage
├── UI/
│   ├── TabControl.cs       # Tab strip (top/side)
│   ├── LayerTreeView.cs    # Layer list with icons
│   ├── SearchBox.cs        # Filter input
│   └── SettingsDialog.cs   # Preferences
└── Settings/
    ├── UserSettings.cs     # Global preferences
    └── DocumentSettings.cs # Per-document groups
```

### Data Storage

**Global Settings** (user preferences):
- Location: `%APPDATA%/LayerTabs/settings.json` (Win) / `~/Library/Application Support/LayerTabs/settings.json` (Mac)
- Contents: tab position, theme, default behavior

**Document Settings** (manual groups, favorites):
- Storage: Rhino document user strings
- Key: `LayerTabs_Groups`, `LayerTabs_Favorites`
- Format: JSON

### Compatibility

The plugin stores data in Rhino's standard user string system. When a file is opened without LayerTabs:
- Layer structure remains unchanged (standard Rhino layers)
- Manual group definitions are preserved but invisible
- No data loss or corruption

---

## UI Design

### Layout Options

**Option A: Tabs on Top**
```
┌─────────────────────────────────────────┐
│ [All] [Park] [Infra] [Monument] [★]     │
├─────────────────────────────────────────┤
│ 🔍 [                              ]     │
├─────────────────────────────────────────┤
│ ▼ perimetercurb                         │
│   ├─ top                    ● 👁 🔒     │
│   ├─ side                   ○ 👁 🔒     │
│   └─ curbstone              ○ 👁 🔒     │
│ ▼ steps                                 │
│   ├─ 01                     ○ 👁 🔒     │
│   └─ 02 - Copy              ○ 👁 🔒     │
└─────────────────────────────────────────┘
```

**Option B: Tabs on Side**
```
┌────────┬────────────────────────────────┐
│ All    │ 🔍 [                    ]      │
│ Park   ├────────────────────────────────┤
│ Infra  │ ▼ perimetercurb                │
│ Monum  │   ├─ top            ● 👁 🔒    │
│ ★      │   ├─ side           ○ 👁 🔒    │
│        │   └─ curbstone      ○ 👁 🔒    │
└────────┴────────────────────────────────┘
```

### Icons

| Icon | Meaning |
|------|---------|
| ● | Current layer |
| ○ | Not current |
| 👁 | Visible (click to hide) |
| 👁‍🗨 | Hidden |
| 🔒 | Locked |
| 🔓 | Unlocked |
| ★ | Favorite |
| ■ | Layer color swatch |

### Color Theme

- Match Rhino's dark theme
- Accent color: user configurable
- High contrast mode option

---

## Settings Dialog

### General Tab
- [ ] Enable LayerTabs panel
- Tab position: [Top ▼] / [Side ▼]
- Default view: [Auto tabs ▼] / [Manual groups ▼] / [Hybrid ▼]

### Appearance Tab
- Theme: [Match Rhino ▼] / [Dark ▼] / [Light ▼]
- Accent color: [■ Pick...]
- Font size: [12 ▼]
- Show layer colors: [✓]

### Behavior Tab
- Double-click action: [Set current ▼] / [Rename ▼]
- Search scope: [Current tab ▼] / [All tabs ▼]
- [ ] Show material names
- [ ] Show object count

### Groups Tab
- Manage manual tab groups
- Import/Export group definitions

---

## API / Commands

### Rhino Commands

| Command | Description |
|---------|-------------|
| `LayerTabs` | Toggle panel visibility |
| `LayerTabsSettings` | Open settings dialog |
| `LayerTabsAddGroup` | Create manual group |
| `LayerTabsRemoveGroup` | Delete manual group |
| `LayerTabsFavorite` | Add/remove layer from favorites |

### Events

The plugin listens to:
- `RhinoDoc.LayerTableEvent` - layer add/delete/modify
- `RhinoDoc.EndOpenDocument` - load document settings
- `RhinoDoc.BeginSaveDocument` - save document settings

---

## Development Roadmap

### Phase 1: Core (MVP)
- [x] Specification
- [ ] Project setup (Visual Studio)
- [ ] Dockable panel registration
- [ ] Basic tab UI (top position)
- [ ] Layer tree view
- [ ] Current layer switching
- [ ] Visibility toggle

### Phase 2: Features
- [ ] Side tab position
- [ ] Search/filter
- [ ] Manual group creation
- [ ] Favorites tab
- [ ] Settings dialog

### Phase 3: Polish
- [ ] Theme support
- [ ] Keyboard shortcuts
- [ ] Context menu
- [ ] Drag & drop reorder

### Phase 4: Distribution
- [ ] Documentation
- [ ] Food4Rhino package
- [ ] License system
- [ ] Mac testing

---

## License & Distribution

### Options

1. **Food4Rhino** (recommended)
   - Built-in license management
   - Automatic updates
   - User reviews & ratings

2. **Gumroad / Custom**
   - More control over pricing
   - Need custom license system

### Pricing Model Ideas

| Tier | Price | Features |
|------|-------|----------|
| Free | $0 | Auto tabs, basic features |
| Pro | $29 | Manual groups, favorites, search |
| Team | $99 | Shared group definitions, support |

---

## File Format Reference

### settings.json (Global)
```json
{
  "version": "1.0",
  "tabPosition": "top",
  "theme": "matchRhino",
  "accentColor": "#50ff78",
  "fontSize": 12,
  "showLayerColors": true,
  "doubleClickAction": "setCurrent",
  "searchScope": "currentTab"
}
```

### Document User Strings

**LayerTabs_Groups:**
```json
{
  "version": "1.0",
  "groups": [
    {
      "name": "Landscape",
      "layerIds": ["guid1", "guid2"],
      "color": "#4CAF50"
    }
  ]
}
```

**LayerTabs_Favorites:**
```json
{
  "version": "1.0",
  "favorites": ["layerGuid1", "layerGuid2"]
}
```

---

## Notes

- Layer structure is never modified by the plugin
- All grouping is virtual (display only)
- Files remain fully compatible with standard Rhino
