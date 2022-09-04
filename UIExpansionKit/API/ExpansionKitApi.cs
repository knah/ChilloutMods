using System;
using System.Collections.Generic;
using MelonLoader;
using UIExpansionKit.API.Layout;
using UIExpansionKit.WebUi;
using UIExpansionKit.WebUi.Elements;

namespace UIExpansionKit.API
{
    public static class ExpansionKitApi
    {
        // internal static readonly Dictionary<ExpandedMenu, CustomLayoutedPageImpl> ExpandedMenus = new();
        internal static readonly Dictionary<string, MainMenuHtmlPageImpl<TableLayout.Param>> SettingPageExtensions = new();

        internal static readonly Dictionary<(string Category, string Entry), SettingVisibilityRegistrationValue> SettingsVisibilities = new();
        internal static readonly Dictionary<(string Category, string Entry), (double min, double max, double snap)> SettingsRanges = new();

        internal static Action<string, string, SettingVisibilityRegistrationValue>
            SettingsVisibilityUpdated = (_, _, _) => { };

        internal class SettingVisibilityRegistrationValue
        {
            private readonly string myCategory;
            private readonly string myEntry;
            internal readonly Func<bool> IsVisible;
            internal event Action OnUpdateVisibility;

            public SettingVisibilityRegistrationValue(string category, string entry, Func<bool> isVisible)
            {
                myCategory = category;
                myEntry = entry;
                IsVisible = isVisible;
            }

            internal void FireUpdateVisibility()
            {
                OnUpdateVisibility?.Invoke();
                SettingsVisibilityUpdated(myCategory, myEntry, this);
            }
        }

        /* This API will be back eventually
        /// <summary>
        /// Returns the interface that can be used to add buttons to expanded menus
        /// </summary>
        /// <param name="menu">Existing menu that the expanded menu will be attached to</param>
        public static ICustomLayoutedMenu GetExpandedMenu(ExpandedMenu menu)
        {
            if (ExpandedMenus.TryGetValue(menu, out var result)) return result;
            
            return ExpandedMenus[menu] = new CustomLayoutedPageImpl(null);
        }*/

        /// <summary>
        /// Returns the interface that can be used to add buttons to settings categories.
        /// </summary>
        /// <param name="categoryName">The category to return the menu for</param>
        public static ICustomLayoutedMenu<TableLayout.Param> GetSettingsCategory(string categoryName)
        {
            if (SettingPageExtensions.TryGetValue(categoryName, out var result)) return result;

            return SettingPageExtensions[categoryName] = new MainMenuHtmlPageImpl<TableLayout.Param>(new TableLayout(1), MainMenuHtmlElementGenerator.Instance);
        }

        internal class ControlRegistration
        {
            public string? Text;
            
            public Action? Action;
            
            public Action<bool>? ToggleAction;
            public Func<bool>? InitialState;

            public override string ToString()
            {
                return $"{Text} IsButton={IsButton} IsToggle={IsToggle}";
            }

            public bool IsButton => Action != null;
            public bool IsToggle => ToggleAction != null;
        }

        /* This API will be back eventually
        /// <summary>
        /// Creates a custom quick menu page.
        /// When shown, the page will be positioned above quick menu, overlapping the main 4x4 grid.
        /// </summary>
        /// <param name="requestedLayout">The layout of the page. If null, a custom layout is assumed - your mod code will need to assign sizes and positions to buttons manually</param>
        public static ICustomShowableLayoutedMenu CreateCustomQuickMenuPage(LayoutDescription? requestedLayout)
        {
            return new CustomQuickMenuPageImpl(requestedLayout);
        }*/
        
        /* This API might be back eventually
        /// <summary>
        /// Creates a custom camera menu page.
        /// When shown, the page will be positioned over the camera expando.
        /// </summary>
        /// <param name="requestedLayout">The layout of the page. If null, a custom layout is assumed - your mod code will need to assign sizes and positions to buttons manually</param>
        public static ICustomShowableLayoutedMenu CreateCustomCameraExpandoPage(LayoutDescription? requestedLayout)
        {
            return new CustomCameraPageImpl(requestedLayout);
        }*/
        
        
        /* This API will be back eventually
        /// <summary>
        /// Creates a custom quick menu expando overlay page.
        /// When shown, the page will be positioned over the quick menu expando.
        /// This overlay is not affected by which quick menu page is shown, or by current page's expando visibility.
        /// </summary>
        /// <param name="requestedLayout">The layout of the page. If null, a custom layout is assumed - your mod code will need to assign sizes and positions to buttons manually</param>
        public static ICustomShowableLayoutedMenu CreateCustomQmExpandoPage(LayoutDescription? requestedLayout)
        {
            return new CustomExpandoOverlayImpl(requestedLayout);
        }*/

        /* This API will be back eventually
        /// <summary>
        /// Registers a custom full menu popup
        /// When shown, the popup will be positioned above full menu, approximately centered.
        /// </summary>
        /// <param name="requestedLayout">The layout of the popup. If null, a custom layout is assumed - your mod code will need to assign sizes and positions to buttons manually</param>
        public static ICustomShowableLayoutedMenu CreateCustomFullMenuPopup(LayoutDescription? requestedLayout)
        {
            return new CustomFullMenuPopupImpl(requestedLayout);
        }*/

        /* This API will be back eventually
        /// <summary>
        /// Hides all custom QM pages and full menu popups that are currently visible. Does not affect expanded menus.
        /// </summary>
        public static void HideAllCustomPopups()
        {
            CustomLayoutedPageWithOwnedMenuImpl.HideAll();
        }*/

        /// <summary>
        /// Registers a visibility callback for a given settings entry.
        /// </summary>
        /// <returns>A delegate that can be called to update visibility of settings entry</returns>
        public static Action RegisterSettingsVisibilityCallback(string category, string setting, Func<bool> isVisible)
        {
            var value = new SettingVisibilityRegistrationValue(category, setting, isVisible);
            SettingsVisibilities[(category, setting)] = value;
            return value.FireUpdateVisibility;
        }

        /// <summary>
        /// Registers a visibility callback for a given settings entry.
        /// </summary>
        /// <returns>A delegate that can be called to update visibility of settings entry</returns>
        public static Action RegisterSettingsVisibilityCallback(MelonPreferences_Entry entry, Func<bool> isVisible) =>
            RegisterSettingsVisibilityCallback(entry.Category.Identifier, entry.Identifier, isVisible);

        /// <summary>
        /// Registers a numerical range for a given settings entry.
        /// The entry will be shown as a slider with corresponding minimum and maximum values instead of a numeric input.
        /// </summary>
        public static void RegisterSettingsRange(string category, string setting, (double Min, double Max) range, double snap = 0)
        {
            SettingsRanges[(category, setting)] = (range.Min, range.Max, snap);
        }

        /// <summary>
        /// Registers a numerical range for a given settings entry.
        /// The entry will be shown as a slider with corresponding minimum and maximum values instead of a numeric input.
        /// </summary>
        public static void RegisterSettingsRange(MelonPreferences_Entry entry, (double Min, double Max) range, double snap = 0) =>
            RegisterSettingsRange(entry.Category.Identifier, entry.Identifier, range);
    }
}