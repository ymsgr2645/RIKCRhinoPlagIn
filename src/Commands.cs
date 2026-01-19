using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;

namespace LayerTabs
{
    [System.Runtime.InteropServices.Guid("B2C3D4E5-F6A7-8901-BCDE-F12345678901")]
    public class LayerTabsCommand : Command
    {
        public override string EnglishName => "LayerTabs";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var panelId = UI.LayerTabsPanel.PanelId;
            var visible = Panels.IsPanelVisible(panelId);

            if (visible)
                Panels.ClosePanel(panelId);
            else
                Panels.OpenPanel(panelId);

            return Result.Success;
        }
    }

    [System.Runtime.InteropServices.Guid("C3D4E5F6-A7B8-9012-CDEF-123456789012")]
    public class LayerTabsSettingsCommand : Command
    {
        public override string EnglishName => "LayerTabsSettings";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var dialog = new UI.SettingsDialog();
            dialog.ShowModal(RhinoEtoApp.MainWindow);
            return Result.Success;
        }
    }

    [System.Runtime.InteropServices.Guid("D4E5F6A7-B8C9-0123-DEF0-234567890123")]
    public class LayerTabsAddGroupCommand : Command
    {
        public override string EnglishName => "LayerTabsAddGroup";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var gs = new GetString();
            gs.SetCommandPrompt("Group name");
            gs.AcceptNothing(false);

            if (gs.Get() != GetResult.String)
                return Result.Cancel;

            var groupName = gs.StringResult();
            if (string.IsNullOrWhiteSpace(groupName))
            {
                RhinoApp.WriteLine("Invalid name");
                return Result.Failure;
            }

            var selectedLayers = new System.Collections.Generic.List<Guid>();

            var go = new GetOption();
            go.SetCommandPrompt("Select layers (Enter when done)");

            foreach (var layer in doc.Layers)
            {
                if (!layer.IsDeleted)
                {
                    go.AddOption(layer.Name.Replace(" ", "_"));
                }
            }

            go.AcceptNothing(true);

            while (true)
            {
                var result = go.Get();
                if (result == GetResult.Nothing)
                    break;

                if (result == GetResult.Option)
                {
                    var layerName = go.Option().EnglishName.Replace("_", " ");
                    var layer = doc.Layers.FindName(layerName);

                    if (layer != null && !selectedLayers.Contains(layer.Id))
                    {
                        selectedLayers.Add(layer.Id);
                        RhinoApp.WriteLine("Added: {0}", layer.Name);
                    }
                }
            }

            if (selectedLayers.Count == 0)
            {
                RhinoApp.WriteLine("No layers selected");
                return Result.Cancel;
            }

            Core.TabManager.Instance?.CreateGroup(groupName, selectedLayers);
            RhinoApp.WriteLine("Group created - OK!");

            return Result.Success;
        }
    }

    [System.Runtime.InteropServices.Guid("E5F6A7B8-C9D0-1234-EF01-345678901234")]
    public class LayerTabsFavoriteCommand : Command
    {
        public override string EnglishName => "LayerTabsFavorite";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var currentLayer = doc.Layers.CurrentLayer;
            if (currentLayer == null)
            {
                RhinoApp.WriteLine("No current layer");
                return Result.Failure;
            }

            var isFavorite = Core.DocumentSettings.ToggleFavorite(doc, currentLayer.Id);
            Core.TabManager.Instance?.Refresh();

            if (isFavorite)
                RhinoApp.WriteLine("* {0} added - OK!", currentLayer.Name);
            else
                RhinoApp.WriteLine("{0} removed - OK!", currentLayer.Name);

            return Result.Success;
        }
    }
}
