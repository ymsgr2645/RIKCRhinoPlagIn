using System;
using Eto.Forms;
using Eto.Drawing;

namespace LayerTabs.UI
{
    public class SettingsDialog : Dialog
    {
        private DropDown _tabPositionDropDown;
        private DropDown _themeDropDown;
        private ColorPicker _accentColorPicker;
        private NumericStepper _fontSizeStepper;
        private CheckBox _showLayerColorsCheck;
        private DropDown _doubleClickDropDown;
        private DropDown _searchScopeDropDown;

        public SettingsDialog()
        {
            Title = "LayerTabs Settings";
            MinimumSize = new Size(400, 450);
            Padding = new Padding(12);
            Resizable = true;

            InitializeComponents();
            LoadSettings();
        }

        private void InitializeComponents()
        {
            _tabPositionDropDown = new DropDown { Items = { "Top", "Side" }, SelectedIndex = 0 };
            _themeDropDown = new DropDown { Items = { "Match Rhino", "Dark", "Light" }, SelectedIndex = 0 };
            _accentColorPicker = new ColorPicker { Value = Color.Parse("#50ff78") };
            _fontSizeStepper = new NumericStepper { MinValue = 8, MaxValue = 24, Value = 12, Increment = 1 };
            _showLayerColorsCheck = new CheckBox { Text = "Show layer colors" };
            _doubleClickDropDown = new DropDown { Items = { "Set as current", "Rename", "Properties" }, SelectedIndex = 0 };
            _searchScopeDropDown = new DropDown { Items = { "Current tab", "All tabs" }, SelectedIndex = 0 };

            var okButton = new Button { Text = "OK" };
            okButton.Click += OnOkClicked;

            var cancelButton = new Button { Text = "Cancel" };
            cancelButton.Click += (s, e) => Close();

            var resetButton = new Button { Text = "Reset" };
            resetButton.Click += OnResetClicked;

            var generalGroup = new GroupBox
            {
                Text = "General",
                Padding = new Padding(8),
                Content = new TableLayout
                {
                    Spacing = new Size(8, 8),
                    Rows =
                    {
                        new TableRow(new Label { Text = "Tab position:" }, _tabPositionDropDown),
                        new TableRow(new Label { Text = "Theme:" }, _themeDropDown),
                    }
                }
            };

            var appearanceGroup = new GroupBox
            {
                Text = "Appearance",
                Padding = new Padding(8),
                Content = new TableLayout
                {
                    Spacing = new Size(8, 8),
                    Rows =
                    {
                        new TableRow(new Label { Text = "Accent color:" }, _accentColorPicker),
                        new TableRow(new Label { Text = "Font size:" }, _fontSizeStepper),
                        new TableRow(null, _showLayerColorsCheck),
                    }
                }
            };

            var behaviorGroup = new GroupBox
            {
                Text = "Behavior",
                Padding = new Padding(8),
                Content = new TableLayout
                {
                    Spacing = new Size(8, 8),
                    Rows =
                    {
                        new TableRow(new Label { Text = "Double-click:" }, _doubleClickDropDown),
                        new TableRow(new Label { Text = "Search scope:" }, _searchScopeDropDown),
                    }
                }
            };

            var buttonLayout = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Items = { resetButton, null, cancelButton, okButton }
            };

            Content = new StackLayout
            {
                Orientation = Orientation.Vertical,
                Spacing = 12,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items = { generalGroup, appearanceGroup, behaviorGroup, null, buttonLayout }
            };
        }

        private void LoadSettings()
        {
            var s = Settings.UserSettings.Current;
            _tabPositionDropDown.SelectedIndex = (int)s.TabPosition;
            _themeDropDown.SelectedIndex = (int)s.Theme;
            try { _accentColorPicker.Value = Color.Parse(s.AccentColor); } catch { }
            _fontSizeStepper.Value = s.FontSize;
            _showLayerColorsCheck.Checked = s.ShowLayerColors;
            _doubleClickDropDown.SelectedIndex = (int)s.DoubleClickAction;
            _searchScopeDropDown.SelectedIndex = (int)s.SearchScope;
        }

        private void SaveSettings()
        {
            var s = Settings.UserSettings.Current;
            s.TabPosition = (Settings.TabPosition)_tabPositionDropDown.SelectedIndex;
            s.Theme = (Settings.ThemeMode)_themeDropDown.SelectedIndex;
            s.AccentColor = _accentColorPicker.Value.ToHex();
            s.FontSize = (int)_fontSizeStepper.Value;
            s.ShowLayerColors = _showLayerColorsCheck.Checked ?? true;
            s.DoubleClickAction = (Settings.DoubleClickAction)_doubleClickDropDown.SelectedIndex;
            s.SearchScope = (Settings.SearchScope)_searchScopeDropDown.SelectedIndex;
            Settings.UserSettings.Save();
        }

        private void OnOkClicked(object sender, EventArgs e)
        {
            SaveSettings();
            Core.TabManager.Instance?.Refresh();
            Close();
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            Settings.UserSettings.Current.Reset();
            LoadSettings();
        }
    }
}
