using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using UIExpansionKit.API;

namespace UIExpansionKit
{
    public static class ExpansionKitSettings
    {
        private static MelonPreferences_Entry<(string Category, string Entry)[]> ourPinnedPrefs;
        internal static MelonPreferences_Entry<float> MainMenuSnapAngle;
        internal static MelonPreferences_Entry<bool> MainMenuInstant;

        internal static void RegisterSettings()
        {
            var category = MelonPreferences.CreateCategory("UIExpansionKit","UI Expansion Kit");
            ourPinnedPrefs = category.CreateEntry("PinnedPrefs", Array.Empty<(string, string)>(), is_hidden: true);

            MainMenuInstant = category.CreateEntry("MenuOpensInstantly", true, "Main menu opens/closes instantly");

            MainMenuSnapAngle = category.CreateEntry("BigMenuSnapAngle", 45f, "Big menu snap angle (slider demo, useless)");
            ExpansionKitApi.RegisterSettingsRange(MainMenuSnapAngle, (0, 90), 15);
        }

        public static void PinPref(string category, string prefName)
        {
            SetPinnedPrefs(ListPinnedPrefs().Concat<(string, string)>(new []{(category, prefName)}).Distinct());
        }
        
        public static void UnpinPref(string category, string prefName)
        {
            SetPinnedPrefs(ListPinnedPrefs().Where(it => it != (category, prefName)));
        }
        
        internal static void SetPinnedPrefs(IEnumerable<(string category, string name)> prefs)
        {
            ourPinnedPrefs.Value = prefs.ToArray();
        }

        public static IEnumerable<(string category, string name)> ListPinnedPrefs()
        {
            return ourPinnedPrefs.Value;
        }
    }
}