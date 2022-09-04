using UIExpansionKit.API;

namespace UIExpansionKit.WebUi.Elements;

internal class MainMenuHtmlElementGenerator : IHtmlElementGenerator
{
    internal static readonly MainMenuHtmlElementGenerator Instance = new();

    private MainMenuHtmlElementGenerator() { }
    
    public string GenerateElement(ExpansionKitApi.ControlRegistration registration, int globalId)
    {
        var gidString = $"data-uix-gid=\"{globalId}\"";
        if (registration.ToggleAction != null)
        {
            var current = (registration.InitialState?.Invoke() ?? false) ? "True" : "False";
            return $"<div class=\"inp_toggle uix_toggle_mark\" {gidString} data-current=\"{current}\"></div>";
        }

        if (registration.Action != null)
        {
            return $"<div class=\"button uix_button_mark\" {gidString} onclick=\"UIX_ButtonClick(this);\">{registration.Text}</div>";
        }

        return $"<div class=\"uix_label_mark\" {gidString}>{registration.Text}</div>";
    }
}