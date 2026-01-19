using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Rhino;
using Rhino.UI;

namespace LayerTabs.UI
{
    [System.Runtime.InteropServices.Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890")]
    public class LayerTabsPanel : Panel, IPanel
    {
        private static Color PanelBackgroundColor = Color.Parse("#2d2d30");

        private StackLayout _tabStrip;
        private TreeGridView _layerTree;
        private SearchBox _searchBox;
        private StackLayout _mainLayout;

        private string _activeTab = "All";
        private List<TabButton> _tabButtons = new List<TabButton>();

        public LayerTabsPanel()
        {
            InitializeComponents();
            Core.TabManager.Instance = new Core.TabManager(this);
        }

        public static Guid PanelId => typeof(LayerTabsPanel).GUID;

        public void PanelShown(uint documentSerialNumber, ShowPanelReason reason)
        {
            RefreshTabs();
            RefreshLayers();
        }

        public void PanelHidden(uint documentSerialNumber, ShowPanelReason reason) { }

        public void PanelClosing(uint documentSerialNumber, bool onCloseDocument) { }

        private void InitializeComponents()
        {
            BackgroundColor = PanelBackgroundColor;

            _searchBox = new SearchBox
            {
                PlaceholderText = "Search layers...",
                Width = -1
            };
            _searchBox.TextChanged += OnSearchChanged;

            _tabStrip = new StackLayout
            {
                Orientation = GetTabOrientation(),
                Spacing = 2,
                Padding = new Padding(4)
            };

            _layerTree = new TreeGridView
            {
                ShowHeader = false,
                AllowMultipleSelection = true
            };

            _layerTree.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<LayerItem, string>(l => l.DisplayName) },
                Expand = true
            });

            _layerTree.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Property<LayerItem, bool?>(l => l.IsVisible) },
                Width = 24
            });

            _layerTree.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Property<LayerItem, bool?>(l => l.IsLocked) },
                Width = 24
            });

            _layerTree.CellDoubleClick += OnLayerDoubleClick;

            var tabPosition = Settings.UserSettings.Current.TabPosition;

            if (tabPosition == Settings.TabPosition.Top)
            {
                _mainLayout = new StackLayout
                {
                    Orientation = Orientation.Vertical,
                    Spacing = 4,
                    Padding = new Padding(4),
                    Items =
                    {
                        new StackLayoutItem(_tabStrip, HorizontalAlignment.Stretch),
                        new StackLayoutItem(_searchBox, HorizontalAlignment.Stretch),
                        new StackLayoutItem(_layerTree, HorizontalAlignment.Stretch, true)
                    }
                };
            }
            else
            {
                var rightPanel = new StackLayout
                {
                    Orientation = Orientation.Vertical,
                    Spacing = 4,
                    Items =
                    {
                        new StackLayoutItem(_searchBox, HorizontalAlignment.Stretch),
                        new StackLayoutItem(_layerTree, HorizontalAlignment.Stretch, true)
                    }
                };

                _mainLayout = new StackLayout
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 4,
                    Padding = new Padding(4),
                    Items =
                    {
                        new StackLayoutItem(_tabStrip, VerticalAlignment.Stretch),
                        new StackLayoutItem(rightPanel, VerticalAlignment.Stretch, true)
                    }
                };
            }

            Content = _mainLayout;
        }

        private Orientation GetTabOrientation()
        {
            return Settings.UserSettings.Current.TabPosition == Settings.TabPosition.Top
                ? Orientation.Horizontal
                : Orientation.Vertical;
        }

        public void RefreshTabs()
        {
            _tabStrip.Items.Clear();
            _tabButtons.Clear();

            var doc = RhinoDoc.ActiveDoc;
            if (doc == null) return;

            AddTab("All", true);

            var topLayers = doc.Layers
                .Where(l => l.ParentLayerId == Guid.Empty && !l.IsDeleted)
                .OrderBy(l => l.SortIndex);

            foreach (var layer in topLayers)
            {
                AddTab(layer.Name, false, layer.Color);
            }

            AddTab("*", false);
        }

        public void RefreshLayers()
        {
            var doc = RhinoDoc.ActiveDoc;
            if (doc == null) return;

            var items = new TreeGridItemCollection();
            var searchText = _searchBox.Text?.ToLower() ?? "";

            IEnumerable<Rhino.DocObjects.Layer> layers;

            if (_activeTab == "All")
            {
                layers = doc.Layers.Where(l => !l.IsDeleted);
            }
            else if (_activeTab == "*")
            {
                var favorites = Core.DocumentSettings.GetFavorites(doc);
                layers = doc.Layers.Where(l => favorites.Contains(l.Id) && !l.IsDeleted);
            }
            else
            {
                var parent = doc.Layers.FindName(_activeTab);
                if (parent != null)
                {
                    layers = GetLayerAndChildren(doc, parent);
                }
                else
                {
                    layers = Enumerable.Empty<Rhino.DocObjects.Layer>();
                }
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                layers = layers.Where(l => l.Name.ToLower().Contains(searchText));
            }

            var rootLayers = layers.Where(l => l.ParentLayerId == Guid.Empty);
            foreach (var layer in rootLayers.OrderBy(l => l.SortIndex))
            {
                items.Add(BuildLayerItem(doc, layer, layers));
            }

            _layerTree.DataStore = items;
        }

        private void AddTab(string name, bool isActive, System.Drawing.Color? color = null)
        {
            var btn = new TabButton(name, isActive, color);
            btn.Clicked += (s, e) => OnTabClicked(name);
            _tabButtons.Add(btn);
            _tabStrip.Items.Add(btn);
        }

        private LayerItem BuildLayerItem(RhinoDoc doc, Rhino.DocObjects.Layer layer, IEnumerable<Rhino.DocObjects.Layer> allLayers)
        {
            var item = new LayerItem(layer);

            var children = allLayers.Where(l => l.ParentLayerId == layer.Id);
            foreach (var child in children.OrderBy(l => l.SortIndex))
            {
                item.Children.Add(BuildLayerItem(doc, child, allLayers));
            }

            return item;
        }

        private IEnumerable<Rhino.DocObjects.Layer> GetLayerAndChildren(RhinoDoc doc, Rhino.DocObjects.Layer parent)
        {
            yield return parent;

            foreach (var child in doc.Layers.Where(l => l.ParentLayerId == parent.Id && !l.IsDeleted))
            {
                foreach (var descendant in GetLayerAndChildren(doc, child))
                {
                    yield return descendant;
                }
            }
        }

        private void OnTabClicked(string tabName)
        {
            _activeTab = tabName;

            foreach (var btn in _tabButtons)
            {
                btn.IsActive = (btn.TabName == tabName);
            }

            RefreshLayers();
        }

        private void OnSearchChanged(object sender, EventArgs e)
        {
            RefreshLayers();
        }

        private void OnLayerDoubleClick(object sender, GridCellMouseEventArgs e)
        {
            if (e.Item is LayerItem item)
            {
                var doc = RhinoDoc.ActiveDoc;
                if (doc == null) return;

                var layer = doc.Layers.FindId(item.LayerId);
                if (layer != null)
                {
                    doc.Layers.SetCurrentLayerIndex(layer.Index, true);
                    RhinoApp.WriteLine("Current layer: {0} - OK!", layer.Name);
                    RefreshLayers();
                }
            }
        }
    }

    public class TabButton : Button
    {
        public string TabName { get; }
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
            }
        }

        public TabButton(string name, bool isActive, System.Drawing.Color? color = null)
        {
            TabName = name;
            _isActive = isActive;

            Text = name.Length > 8 ? name.Substring(0, 7) + ".." : name;
            MinimumSize = new Size(60, 28);
        }
    }

    public class LayerItem : TreeGridItem
    {
        public Guid LayerId { get; }
        public string DisplayName { get; }
        public bool? IsVisible { get; set; }
        public bool? IsLocked { get; set; }
        public bool IsCurrent { get; }
        public System.Drawing.Color LayerColor { get; }

        public LayerItem(Rhino.DocObjects.Layer layer)
        {
            LayerId = layer.Id;
            DisplayName = layer.Name;
            IsVisible = layer.IsVisible;
            IsLocked = layer.IsLocked;
            IsCurrent = (layer.Index == RhinoDoc.ActiveDoc?.Layers.CurrentLayerIndex);
            LayerColor = layer.Color;
        }
    }
}
