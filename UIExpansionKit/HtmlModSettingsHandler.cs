using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using HarmonyLib;
using MelonLoader;
using UIExpansionKit.API;
using UIExpansionKit.WebUi;

namespace UIExpansionKit;

#nullable enable

public class HtmlModSettingsHandler
{
    internal void PerformInjection(CohtmlView mainMenuView)
    {
        FruityLogger.Msg($"Preparing to inject menu into {mainMenuView.gameObject.name}");
        mainMenuView.Listener.ReadyForBindings += () => { DoBinds(mainMenuView); };
        if (mainMenuView.View.IsReadyForBindings())
            DoBinds(mainMenuView);

        ExpansionKitApi.SettingsVisibilityUpdated += (category, entry, visibility) =>
        {
            SendVisibility(mainMenuView, category, entry, visibility);
        };
    }

    private static void SendVisibility(CohtmlView mainMenuView, string category, string entry,
        ExpansionKitApi.SettingVisibilityRegistrationValue visibility)
    {
        mainMenuView.View.TriggerEvent("UixSettingVisibilityUpdated", category, entry, visibility.IsVisible());
    }

    private void DoBinds(CohtmlView mainMenuView)
    {
        FruityLogger.Msg("Adding settings binds");
        mainMenuView.View.RegisterForEvent("UixSetBoolSetting", SetBoolSettingFromJs);
        mainMenuView.View.RegisterForEvent("UixSetDropdownSetting", SetDropdownSettingFromJs);
        mainMenuView.View.RegisterForEvent("UixSetSlider", SetSliderFromJs);
        mainMenuView.View.RegisterForEvent("UixSetNumber", SetNumberFromJs);
        mainMenuView.View.RegisterForEvent("UixSetString", SetStringFromJs);

        mainMenuView.View.RegisterForEvent("UixResubmitSettingVisibilities", ResubmitSettingVisibilities);
        
        mainMenuView.View.BindCall("UixGetSettingsHtml", GetHtmlForCategoriesListFromJs);

        FruityLogger.Msg("Executing injected JS");
        mainMenuView.View.ExecuteScript(GetInjectorJs());
    }

    private void ResubmitSettingVisibilities()
    {
        foreach (var kvp in ExpansionKitApi.SettingsVisibilities)
            SendVisibility(ViewManager.Instance.gameMenuView, kvp.Key.Category, kvp.Key.Entry, kvp.Value);

        foreach (var category in MelonPreferences.Categories)
        {
            foreach (var entry in category.Entries)
            {
                if (entry.IsHidden || entry is not MelonPreferences_Entry<string> stringEntry) continue;

                ViewManager.Instance.gameMenuView.View.TriggerEvent("UixSettingValueUpdated", category.Identifier,
                    entry.Identifier, stringEntry.Value);
            }
        }
    }

    private string GetHtmlForCategoriesListFromJs()
    {
        FruityLogger.Msg("JS requested HTML data");
        var builder = new StringBuilder();

        foreach (var s in GetModSettingListPage())
            builder.AppendLine(s);

        FruityLogger.Msg($"Returning {builder.Length} chars to js");

        return builder.ToString();
    }

    private string GetInjectorJs()
    {
        using var sourceStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("UIExpansionKit.WebUi.Js.main-menu-inject.js")!;
        using var targetStream = new MemoryStream();

        sourceStream.CopyTo(targetStream);

        return Encoding.UTF8.GetString(targetStream.ToArray());
    }

    private IEnumerable<string> GetModSettingListPage()
    {
        yield return "<div id=\"uix-settings\" class=\"content hidden\">";

        yield return "<div class=\"list-filter\">";
        yield return "<h1>Mod Settings</h1>";
        yield return "<div class=\"scroll-content\" style=\"top: 4em; bottom: 3em;\">";
        yield return "<div class=\"filter-content\">";

        var categories = GetViableCategories().ToList();

        foreach (var category in categories)
            yield return GetModSettingCatHtml(category);

        yield return "</div></div></div>";

        yield return "<div class=\"list-content\">";

        foreach (var category in categories)
        foreach (var s in GetModSettingCatBody(category))
            yield return s;

        yield return "</div>";

        yield return "</div>";
    }

    private string GetModSettingCatHtml(MelonPreferences_Category category)
    {
        var catName = category.DisplayName ?? category.Identifier;
        return
            $"<div class=\"filter-option button\" onclick=\"switchUixCat('UIX-cat-{category.Identifier}', this)\">{catName}</div>";
    }

    private IEnumerable<string> GetModSettingCatBody(MelonPreferences_Category category)
    {
        yield return $"<div id=\"UIX-cat-{category.Identifier}\" class=\"settings-categorie\">";

        foreach (var entry in category.Entries)
        {
            if (!EntryCanBeShown(entry)) continue;

            string? inputEntry = null;

            if (entry is MelonPreferences_Entry<bool> boolEntry)
                inputEntry = HtmlEntriesGenerator.GetToggleHtml(boolEntry);
            else if (entry is MelonPreferences_Entry<string> stringEntry)
                inputEntry = HtmlEntriesGenerator.GetTextInputEntry(stringEntry);
            else if (entry.GetReflectedType().IsEnum)
            {
                inputEntry = (string)AccessTools.Method(typeof(HtmlEntriesGenerator),
                        nameof(HtmlEntriesGenerator.GetDropdownHtml))
                    .MakeGenericMethod(entry.GetReflectedType()).Invoke(this, new object[] { entry });
            }
            else if (entry.GetReflectedType().IsPrimitive)
            {
                inputEntry = (string)AccessTools.Method(typeof(HtmlEntriesGenerator),
                        nameof(HtmlEntriesGenerator.GetNumericEntryHtml))
                    .MakeGenericMethod(entry.GetReflectedType()).Invoke(this, new object[] { entry });
            }

            if (inputEntry == null) continue;

            yield return "<div class=\"row-wrapper\">";
            yield return $"<div class=\"option-caption\">{entry.DisplayName ?? entry.Identifier}</div>";
            yield return "<div class=\"option-input\">";

            yield return inputEntry;

            yield return "</div></div>";
        }

        if (ExpansionKitApi.SettingPageExtensions.TryGetValue(category.Identifier, out var extraElements))
        {
            yield return extraElements.GetHtml();
        }

        yield return "</div>";
    }

    private MelonPreferences_Entry? GetEntry(string categoryName, string entryName)
    {
        var category = MelonPreferences.Categories.SingleOrDefault(it => it.Identifier == categoryName);
        if (category == null) return null;

        var entry = category.Entries.SingleOrDefault(it => it.Identifier == entryName);
        if (entry == null) return null;

        return entry;
    }

    private void SetDropdownSettingFromJs(string categoryName, string entryName, int index)
    {
        FruityLogger.Msg($"Got bool toggle call for {categoryName}/{entryName} = {index}");
        var setter = HtmlEntriesGenerator.GetDropdownSetterFor(categoryName, entryName);

        setter?.Invoke(index);
    }

    private void SetSliderFromJs(string categoryName, string entryName, double value, bool write)
    {
        FruityLogger.Msg($"Got bool toggle call for {categoryName}/{entryName} = {value}, {write}");
        var entry = GetEntry(categoryName, entryName);
        if (entry == null) return;

        switch (entry)
        {
            case MelonPreferences_Entry<byte> byteEntry:
                byteEntry.Value = (byte)value;
                break;
            case MelonPreferences_Entry<sbyte> byteEntry:
                byteEntry.Value = (sbyte)value;
                break;
            case MelonPreferences_Entry<short> byteEntry:
                byteEntry.Value = (short)value;
                break;
            case MelonPreferences_Entry<ushort> byteEntry:
                byteEntry.Value = (ushort)value;
                break;
            case MelonPreferences_Entry<int> byteEntry:
                byteEntry.Value = (int)value;
                break;
            case MelonPreferences_Entry<uint> byteEntry:
                byteEntry.Value = (uint)value;
                break;
            case MelonPreferences_Entry<long> byteEntry:
                byteEntry.Value = (long)value;
                break;
            case MelonPreferences_Entry<ulong> byteEntry:
                byteEntry.Value = (ulong)value;
                break;
            case MelonPreferences_Entry<float> byteEntry:
                byteEntry.Value = (float)value;
                break;
            case MelonPreferences_Entry<double> byteEntry:
                byteEntry.Value = value;
                break;
            default:
                FruityLogger.Msg($"Unexpected numeric entry type: {entry.GetType()}");
                break;
        }

        if (write)
            entry.Category.SaveToFile(false);
    }

    private void SetNumberFromJs(string category, string entry, double value) =>
        SetSliderFromJs(category, entry, value, true);

    private void SetStringFromJs(string category, string entryName, string value)
    {
        var entry = GetEntry(category, entryName);
        if (entry is not MelonPreferences_Entry<string> stringEntry) return;
        stringEntry.Value = value;
        entry.Category.SaveToFile(false);
    }

    private void SetBoolSettingFromJs(string categoryName, string entryName, bool set)
    {
        FruityLogger.Msg($"Got bool toggle call for {categoryName}/{entryName} = {set}");
        var entry = GetEntry(categoryName, entryName);
        if (entry is not MelonPreferences_Entry<bool> boolEntry) return;

        boolEntry.Value = set;
        boolEntry.Category.SaveToFile(false);
    }

    private bool EntryCanBeShown(MelonPreferences_Entry entry)
    {
        if (entry.IsHidden) return false;
        var entryType = entry.GetReflectedType();
        return entryType == typeof(bool) || entryType.IsEnum || entryType.IsPrimitive || entryType == typeof(string);
    }

    private IEnumerable<MelonPreferences_Category> GetViableCategories()
    {
        return MelonPreferences.Categories.Where(it => !it.IsHidden && (it.Entries.Any(EntryCanBeShown) || ExpansionKitApi.SettingPageExtensions.ContainsKey(it.Identifier))).OrderBy(x=>x.DisplayName);
    }
}