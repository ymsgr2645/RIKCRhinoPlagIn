using System;
using Eto.Drawing;

namespace LayerTabs
{
    public static class Resources
    {
        public static System.Drawing.Icon GetPanelIcon()
        {
            return null;
        }

        public static class Colors
        {
            public static Color Accent => Color.Parse("#50ff78");
            public static Color Background => Color.Parse("#2d2d30");
            public static Color TabActive => Color.Parse("#3e3e42");
            public static Color TabInactive => Color.Parse("#252526");
            public static Color Text => Color.Parse("#ffffff");
            public static Color TextDim => Color.Parse("#a0a0a0");
            public static Color Border => Color.Parse("#3f3f46");
            public static Color White => Color.Parse("#ffffff");
            public static Color Gray => Color.Parse("#808080");
            public static Color Transparent => Color.FromArgb(0, 0, 0, 0);
        }
    }
}
