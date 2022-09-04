namespace UIExpansionKit.WebUi.Events;

public interface IControlFeedbackSink
{
    void ButtonClick(int globalId);
    void ToggleClick(int globalId, bool state);
}