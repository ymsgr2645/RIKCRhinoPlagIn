using System;
using System.IO;
using Newtonsoft.Json;

namespace LayerTabs.Settings
{
    public enum TabPosition { Top, Side }
    public enum ThemeMode { MatchRhino, Dark, Light }
    public enum DoubleClickAction { SetCurrent, Rename, Properties }
    public enum SearchScope { CurrentTab, AllTabs }

    public class UserSettings
    {
        private static UserSettings _current;
        public static UserSettings Current => _current ?? (_current = new UserSettings());

        [JsonProperty("version")]
        public string Version { get; set; } = "1.0";

        [JsonProperty("tabPosition")]
        public TabPosition TabPosition { get; set; } = TabPosition.Top;

        [JsonProperty("theme")]
        public ThemeMode Theme { get; set; } = ThemeMode.MatchRhino;

        [JsonProperty("accentColor")]
        public string AccentColor { get; set; } = "#50ff78";

        [JsonProperty("fontSize")]
        public int FontSize { get; set; } = 12;

        [JsonProperty("showLayerColors")]
        public bool ShowLayerColors { get; set; } = true;

        [JsonProperty("doubleClickAction")]
        public DoubleClickAction DoubleClickAction { get; set; } = DoubleClickAction.SetCurrent;

        [JsonProperty("searchScope")]
        public SearchScope SearchScope { get; set; } = SearchScope.CurrentTab;

        private static string SettingsPath =>
            Path.Combine(LayerTabsPlugin.SettingsFolder, "settings.json");

        public static void Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    _current = JsonConvert.DeserializeObject<UserSettings>(json);
                }
            }
            catch { }

            if (_current == null)
                _current = new UserSettings();
        }

        public static void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_current, Formatting.Indented);
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }

        public void Reset()
        {
            TabPosition = TabPosition.Top;
            Theme = ThemeMode.MatchRhino;
            AccentColor = "#50ff78";
            FontSize = 12;
            ShowLayerColors = true;
            DoubleClickAction = DoubleClickAction.SetCurrent;
            SearchScope = SearchScope.CurrentTab;
        }
    }
}
