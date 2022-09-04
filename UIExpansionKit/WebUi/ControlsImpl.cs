using UIExpansionKit.API.Controls;
using UIExpansionKit.WebUi.Events;
using UnityEngine;

namespace UIExpansionKit.WebUi;

internal interface IControlImplDetails
{
    void ResyncParams();
}

internal class BaseHtmlControl : IMenuControl, IControlImplDetails
{
    public readonly int GlobalId;
    protected readonly IControlEventSink Sink;
    private bool myVisible;

    public BaseHtmlControl(int globalId, IControlEventSink sink)
    {
        GlobalId = globalId;
        Sink = sink;
    }

    public bool Visible
    {
        get => myVisible;
        set
        {
            myVisible = value;
            Sink.SetVisible(GlobalId, value);
        }
    }

    public virtual void ResyncParams()
    {
        Sink.SetVisible(GlobalId, myVisible);
    }
}

internal class BaseControlWithText : BaseHtmlControl, IMenuButton, IMenuLabel
{
    private string myText;
    private TextAnchor myAnchor;

    public BaseControlWithText(int globalId, IControlEventSink sink) : base(globalId, sink)
    {
    }

    public string Text
    {
        get => myText;
        set
        {
            myText = value;
            Sink.SetText(GlobalId, value);
        }
    }

    public TextAnchor Anchor
    {
        get => myAnchor;
        set
        {
            myAnchor = value;
            Sink.SetTextAnchor(GlobalId, value);
        }
    }

    public override void ResyncParams()
    {
        base.ResyncParams();
        Sink.SetText(GlobalId, myText);
        Sink.SetTextAnchor(GlobalId, myAnchor);
    }
}

internal class ToggleImpl : BaseControlWithText, IMenuToggle
{
    private bool mySelected;

    public ToggleImpl(int globalId, IControlEventSink sink) : base(globalId, sink)
    {
    }

    public bool Selected
    {
        get => mySelected;
        set
        {
            mySelected = value;
            Sink.SetSelected(GlobalId, value);
        }
    }

    public override void ResyncParams()
    {
        base.ResyncParams();
        Sink.SetSelected(GlobalId, mySelected);
    }
}