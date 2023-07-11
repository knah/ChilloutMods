using System;
using System.Collections.Generic;
using System.Linq;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using MelonLoader;
using UIExpansionKit.API;

namespace UIExpansionKit.WebUi;

public class HtmlEntriesGenerator
{
    private static readonly Dictionary<(string, string), Action<int>> EnumSetters = new();
    private static readonly List<Action> UnsubDelegates = new();

    internal static void Reset()
    {
        foreach (var unsubDelegate in UnsubDelegates) unsubDelegate();
        UnsubDelegates.Clear();
        EnumSetters.Clear();
    }
    
    internal static void DoSubToEntryChange<T>(MelonPreferences_Entry<T> entry, Action<T, T> handler)
    {
        entry.OnValueChanged += handler;
        UnsubDelegates.Add(() => entry.OnValueChanged -= handler);
    }
    
    internal static string GetDropdownHtml<T>(MelonPreferences_Entry<T> entry) where T: struct, Enum
    {
        var options = EnumPrefUtil.GetEnumSettingOptions<T>();
        return GetDropdownHtmlImpl(entry, options);
    }

    private static string GetCommonData<T>(MelonPreferences_Entry<T> entry)
    {
        return $" data-uix-cat=\"{entry.Category.Identifier}\" data-uix-entry=\"{entry.Identifier}\"";
    }

    private static string GetDropdownHtmlImpl<T>(MelonPreferences_Entry<T> entry, List<(T SettingsValue, string DisplayName)> options)
    {
        var category = entry.Category;
        EnumSetters[(category.Identifier, entry.Identifier)] = i =>
        {
            entry.Value = options[i].SettingsValue;
            category.SaveToFile();
        };

        DoSubToEntryChange(entry, (_, newValue) =>
        {
            ViewManager.Instance.gameMenuView.View.TriggerEvent("UixSettingValueUpdated", category.Identifier,
                entry.Identifier, options.FindIndex(it => Equals(it.SettingsValue, newValue)));
        });

        var dataOptions = options.Select((pair, idx) => $"{idx}:{pair.DisplayName.Replace(',', ' ').Replace(':', ' ')}");
        var optionsListString = string.Join(",", dataOptions);
        var current = options.FindIndex(it => it.SettingsValue.Equals(entry.Value));
        return $"<div {GetCommonData(entry)} class=\"inp_dropdown\" data-options=\"{optionsListString}\" data-current=\"{current}\"></div>";
    }

    internal static string GetToggleHtml(MelonPreferences_Entry<bool> entry)
    {
        DoSubToEntryChange(entry, (_, newValue) =>
        {
            ViewManager.Instance.gameMenuView.View.TriggerEvent("UixSettingValueUpdated", entry.Category.Identifier,
                entry.Identifier, newValue ? "True" : "False");
        });

        return $"<div {GetCommonData(entry)} class=\"inp_toggle\" data-current=\"{entry.Value}\" ></div>";
    }

    private static string GetNumericSliderAttribute<T>(MelonPreferences_Entry<T> entry, (double min, double max, double snap) range) where T: struct
    {
        var type = typeof(T);
        var isFloat = type == typeof(float) || type == typeof(double);
        var stepQualifier = isFloat ? $" data-stepSize=\"{range.snap}\"" : $" data-stepSize=\"{Math.Max(1, (int)range.snap)}\" ";
        return $"<div {GetCommonData(entry)} class=\"inp_slider\" data-current=\"{entry.Value}\" data-min=\"{range.min}\" data-max=\"{range.max}\" {stepQualifier}></div>";
    }

    internal static string GetTextInputEntry(MelonPreferences_Entry<string> entry)
    {
        return $"<input {GetCommonData(entry)} type=\"text\" class=\"inp_search\" data-submit=\"UIX_StringSettingUpdated();\" onclick=\"UIX_StringOpenKeyboard(this);\" />";
    }

    internal static string GetNumericEntryHtml<T>(MelonPreferences_Entry<T> entry)
        where T : struct
    {
        if (ExpansionKitApi.SettingsRanges.TryGetValue((entry.Category.Identifier, entry.Identifier), out var range))
        {
            return GetNumericSliderAttribute(entry, range);
        }
        
        var type = typeof(T);
        var isFloat = type == typeof(float) || type == typeof(double);
        var valueType = isFloat ? " data-mode=\"float\" " : " data-mode=\"int\" ";

        return $"<div {GetCommonData(entry)} class=\"inp_number\" data-caption=\"Value\" data-current=\"{entry.Value}\" {valueType}></div>";
    }

    public static Action<int>? GetDropdownSetterFor(string categoryName, string entryName)
    {
        EnumSetters.TryGetValue((categoryName, entryName), out var setter);
        return setter;
    }
}