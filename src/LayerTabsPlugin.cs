using System;
using System.IO;
using Rhino;
using Rhino.PlugIns;
using Rhino.UI;

namespace LayerTabs
{
    public class LayerTabsPlugin : PlugIn
    {
        public static LayerTabsPlugin Instance { get; private set; }

        public LayerTabsPlugin()
        {
            Instance = this;
        }

        public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            Settings.UserSettings.Load();

            var panelType = typeof(UI.LayerTabsPanel);
            Panels.RegisterPanel(this, panelType, "LayerTabs", Resources.GetPanelIcon());

            RhinoDoc.EndOpenDocument += OnDocumentOpened;
            RhinoDoc.BeginSaveDocument += OnDocumentSaving;
            RhinoDoc.CloseDocument += OnDocumentClosed;
            RhinoDoc.LayerTableEvent += OnLayerTableChanged;

            RhinoApp.WriteLine("LayerTabs loaded - OK!");
            return LoadReturnCode.Success;
        }

        protected override void OnShutdown()
        {
            RhinoDoc.EndOpenDocument -= OnDocumentOpened;
            RhinoDoc.BeginSaveDocument -= OnDocumentSaving;
            RhinoDoc.CloseDocument -= OnDocumentClosed;
            RhinoDoc.LayerTableEvent -= OnLayerTableChanged;

            Settings.UserSettings.Save();
            base.OnShutdown();
        }

        private void OnDocumentOpened(object sender, DocumentOpenEventArgs e)
        {
            Core.DocumentSettings.Load(e.Document);
            Core.TabManager.Instance?.Refresh();
        }

        private void OnDocumentSaving(object sender, DocumentSaveEventArgs e)
        {
            Core.DocumentSettings.Save(e.Document);
        }

        private void OnDocumentClosed(object sender, DocumentEventArgs e)
        {
            Core.TabManager.Instance?.Clear();
        }

        private void OnLayerTableChanged(object sender, Rhino.DocObjects.Tables.LayerTableEventArgs e)
        {
            Core.TabManager.Instance?.OnLayerChanged(e);
        }

        public static string SettingsFolder
        {
            get
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "LayerTabs");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                return folder;
            }
        }
    }
}
