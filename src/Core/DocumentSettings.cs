using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Rhino;

namespace LayerTabs.Core
{
    public static class DocumentSettings
    {
        private const string GroupsKey = "LayerTabs_Groups";
        private const string FavoritesKey = "LayerTabs_Favorites";

        public static void Load(RhinoDoc doc)
        {
            if (doc == null) return;

            try
            {
                var groupsJson = doc.Strings.GetValue(GroupsKey);
                if (!string.IsNullOrEmpty(groupsJson))
                {
                    var data = JsonConvert.DeserializeObject<GroupsData>(groupsJson);
                    TabManager.Instance?.LoadGroups(data?.Groups);
                }
            }
            catch { }
        }

        public static void Save(RhinoDoc doc)
        {
            if (doc == null) return;

            try
            {
                var groups = TabManager.Instance?.GetManualGroups();
                if (groups != null && groups.Count > 0)
                {
                    var data = new GroupsData { Version = "1.0", Groups = groups.ToList() };
                    var json = JsonConvert.SerializeObject(data);
                    doc.Strings.SetString(GroupsKey, json);
                }
                else
                {
                    doc.Strings.Delete(GroupsKey);
                }
            }
            catch { }
        }

        public static HashSet<Guid> GetFavorites(RhinoDoc doc)
        {
            if (doc == null) return new HashSet<Guid>();

            try
            {
                var json = doc.Strings.GetValue(FavoritesKey);
                if (!string.IsNullOrEmpty(json))
                {
                    var data = JsonConvert.DeserializeObject<FavoritesData>(json);
                    return new HashSet<Guid>(data?.Favorites ?? new List<Guid>());
                }
            }
            catch { }

            return new HashSet<Guid>();
        }

        public static void AddFavorite(RhinoDoc doc, Guid layerId)
        {
            if (doc == null) return;
            var favorites = GetFavorites(doc);
            favorites.Add(layerId);
            SaveFavorites(doc, favorites);
        }

        public static void RemoveFavorite(RhinoDoc doc, Guid layerId)
        {
            if (doc == null) return;
            var favorites = GetFavorites(doc);
            favorites.Remove(layerId);
            SaveFavorites(doc, favorites);
        }

        public static bool ToggleFavorite(RhinoDoc doc, Guid layerId)
        {
            if (doc == null) return false;

            var favorites = GetFavorites(doc);
            bool isFavorite;

            if (favorites.Contains(layerId))
            {
                favorites.Remove(layerId);
                isFavorite = false;
            }
            else
            {
                favorites.Add(layerId);
                isFavorite = true;
            }

            SaveFavorites(doc, favorites);
            return isFavorite;
        }

        private static void SaveFavorites(RhinoDoc doc, HashSet<Guid> favorites)
        {
            try
            {
                if (favorites.Count > 0)
                {
                    var data = new FavoritesData { Version = "1.0", Favorites = favorites.ToList() };
                    var json = JsonConvert.SerializeObject(data);
                    doc.Strings.SetString(FavoritesKey, json);
                }
                else
                {
                    doc.Strings.Delete(FavoritesKey);
                }
            }
            catch { }
        }

        private class GroupsData
        {
            [JsonProperty("version")] public string Version { get; set; }
            [JsonProperty("groups")] public List<TabGroup> Groups { get; set; }
        }

        private class FavoritesData
        {
            [JsonProperty("version")] public string Version { get; set; }
            [JsonProperty("favorites")] public List<Guid> Favorites { get; set; }
        }
    }
}
