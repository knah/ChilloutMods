using UIExpansionKit.API;

namespace UIExpansionKit.WebUi.Elements;

internal interface IHtmlElementGenerator
{
    string GenerateElement(ExpansionKitApi.ControlRegistration registration, int globalId);
}