using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.DocObjects.Tables;

namespace LayerTabs.Core
{
    public class TabManager
    {
        public static TabManager Instance { get; set; }

        private readonly UI.LayerTabsPanel _panel;
        private List<TabGroup> _manualGroups = new List<TabGroup>();

        public TabManager(UI.LayerTabsPanel panel)
        {
            _panel = panel;
        }

        public void Refresh()
        {
            _panel?.RefreshTabs();
            _panel?.RefreshLayers();
        }

        public void Clear()
        {
            _manualGroups.Clear();
        }

        public void OnLayerChanged(LayerTableEventArgs e)
        {
            Refresh();
        }

        public TabGroup CreateGroup(string name, IEnumerable<Guid> layerIds)
        {
            var group = new TabGroup
            {
                Id = Guid.NewGuid(),
                Name = name,
                LayerIds = layerIds.ToList(),
                Color = "#50ff78"
            };

            _manualGroups.Add(group);
            Refresh();

            return group;
        }

        public bool RemoveGroup(Guid groupId)
        {
            var removed = _manualGroups.RemoveAll(g => g.Id == groupId) > 0;
            if (removed) Refresh();
            return removed;
        }

        public IReadOnlyList<TabGroup> GetManualGroups() => _manualGroups.AsReadOnly();

        public void LoadGroups(List<TabGroup> groups)
        {
            _manualGroups = groups ?? new List<TabGroup>();
        }

        public List<AutoTab> GetAutoTabs()
        {
            var doc = RhinoDoc.ActiveDoc;
            if (doc == null) return new List<AutoTab>();

            return doc.Layers
                .Where(l => l.ParentLayerId == Guid.Empty && !l.IsDeleted)
                .OrderBy(l => l.SortIndex)
                .Select(l => new AutoTab
                {
                    Name = l.Name,
                    LayerId = l.Id,
                    Color = string.Format("#{0:X2}{1:X2}{2:X2}", l.Color.R, l.Color.G, l.Color.B),
                    ChildCount = doc.Layers.Count(c => c.ParentLayerId == l.Id && !c.IsDeleted)
                })
                .ToList();
        }
    }

    public class TabGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> LayerIds { get; set; } = new List<Guid>();
        public string Color { get; set; }
    }

    public class AutoTab
    {
        public string Name { get; set; }
        public Guid LayerId { get; set; }
        public string Color { get; set; }
        public int ChildCount { get; set; }
    }
}
