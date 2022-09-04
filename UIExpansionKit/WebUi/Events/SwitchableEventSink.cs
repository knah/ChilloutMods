using UnityEngine;

namespace UIExpansionKit.WebUi.Events;

#nullable enable

public class SwitchableEventSink : IControlEventSink
{
    public IControlEventSink? Target;

    public void SetVisible(int globalId, bool visible) => Target?.SetVisible(globalId, visible);
    public void SetText(int globalId, string text) => Target?.SetText(globalId, text);
    public void SetTextAnchor(int globalId, TextAnchor anchor) => Target?.SetTextAnchor(globalId, anchor);
    public void SetSelected(int globalId, bool selected) => Target?.SetSelected(globalId, selected);

    public IControlFeedbackSink? FeedbackSink
    {
        get => Target?.FeedbackSink;
        set {
            if (Target != null) Target.FeedbackSink = value;
        }
    }
}