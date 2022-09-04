using UnityEngine;

namespace UIExpansionKit.WebUi.Events;

public interface IControlEventSink
{
    void SetVisible(int globalId, bool visible);
    void SetText(int globalId, string text);
    void SetTextAnchor(int globalId, TextAnchor anchor);
    void SetSelected(int globalId, bool selected);
    
    IControlFeedbackSink? FeedbackSink { get; set; }
}