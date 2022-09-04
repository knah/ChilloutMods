using UIExpansionKit.API.Layout;
using UIExpansionKit.WebUi.Elements;

namespace UIExpansionKit.WebUi;

internal class MainMenuHtmlPageImpl<TLayoutParam> : BaseHtmlMenuPage<TLayoutParam> where TLayoutParam : struct
{
    public MainMenuHtmlPageImpl(IHtmlLayout<TLayoutParam> layout, IHtmlElementGenerator generator) : base(layout, generator)
    {
    }
}