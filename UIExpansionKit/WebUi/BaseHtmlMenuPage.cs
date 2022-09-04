using System;
using System.Collections.Generic;
using UIExpansionKit.API;
using UIExpansionKit.API.Controls;
using UIExpansionKit.API.Layout;
using UIExpansionKit.WebUi.Elements;
using UIExpansionKit.WebUi.Events;

namespace UIExpansionKit.WebUi;

internal abstract class BaseHtmlMenuPage<TLayoutParam> : ICustomLayoutedMenu<TLayoutParam>, IDisposable, IControlFeedbackSink where TLayoutParam : struct
{
    private readonly List<(ExpansionKitApi.ControlRegistration, IControlImplDetails)> myControlRegistrations = new();
    private readonly Dictionary<int, ExpansionKitApi.ControlRegistration> myControlHandlerDelegates = new();
    private readonly IHtmlLayout<TLayoutParam> myLayout;
    private readonly SwitchableEventSink mySwitchableSink = new();
    private readonly IHtmlElementGenerator myGenerator;

    public IControlEventSink? Sink
    {
        get => mySwitchableSink.Target;
        set
        {
            var lastTarget = mySwitchableSink.Target;
            if (lastTarget != null && lastTarget.FeedbackSink == this) 
                lastTarget.FeedbackSink = null;
            
            mySwitchableSink.Target = value;
            if (value != null) value.FeedbackSink = this;
        }
    }

    protected BaseHtmlMenuPage(IHtmlLayout<TLayoutParam> layout, IHtmlElementGenerator generator)
    {
        myLayout = layout;
        myGenerator = generator;
    }

    public void ButtonClick(int globalId)
    {
        if (!myControlHandlerDelegates.TryGetValue(globalId, out var registration)) return;
        registration.Action?.Invoke();
    }

    public void ToggleClick(int globalId, bool state)
    {
        if (!myControlHandlerDelegates.TryGetValue(globalId, out var registration)) return;
        registration.ToggleAction?.Invoke(state);
    }

    public virtual void Dispose()
    {
        Sink = null;
    }

    public IMenuButton AddSimpleButton(string text, Action onClick, TLayoutParam? param)
    {
        return AddSimpleButton(text, _ => onClick(), param);
    }

    public IMenuButton AddSimpleButton(string text, Action<IMenuButton> onClick, TLayoutParam? param)
    {
        var controlImpl = new BaseControlWithText(GlobalControlId.Next(), mySwitchableSink);
        var controlRegistration = new ExpansionKitApi.ControlRegistration {Text = text, Action = () => onClick(controlImpl)};
        AddControl(param, controlRegistration, controlImpl);
        return controlImpl;
    }

    public IMenuToggle AddToggleButton(string text, Action<bool> onClick, Func<bool>? getInitialState = null, TLayoutParam? param = null)
    {
        var controlImpl = new ToggleImpl(GlobalControlId.Next(), mySwitchableSink);
        var controlRegistration = new ExpansionKitApi.ControlRegistration {Text = text, InitialState = getInitialState, ToggleAction = onClick};
        AddControl(param, controlRegistration, controlImpl);
        return controlImpl;
    }

    public IMenuLabel AddLabel(string text, TLayoutParam? param)
    {
        var controlImpl = new BaseControlWithText(GlobalControlId.Next(), mySwitchableSink);
        var controlRegistration = new ExpansionKitApi.ControlRegistration {Text = text};
        AddControl(param, controlRegistration, controlImpl);
        return controlImpl;
    }

    public IMenuControl AddSpacer(TLayoutParam? param)
    {
        var controlImpl = new BaseHtmlControl(GlobalControlId.Next(), mySwitchableSink);
        var controlRegistration = new ExpansionKitApi.ControlRegistration();
        AddControl(param, controlRegistration, controlImpl);
        return controlImpl;
    }

    private void AddControl(TLayoutParam? param, ExpansionKitApi.ControlRegistration controlRegistration, BaseHtmlControl controlImpl)
    {
        myLayout.AddControl(myGenerator.GenerateElement(controlRegistration, controlImpl.GlobalId), param);
        myControlRegistrations.Add((controlRegistration, controlImpl));
        myControlHandlerDelegates[controlImpl.GlobalId] = controlRegistration;
    }

    internal string GetHtml() => myLayout.GetHtml();
}