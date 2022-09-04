using System.IO;
using System.Reflection;
using System.Text;
using cohtml;
using UnityEngine;

namespace UIExpansionKit.WebUi.Events;

public class ViewEventSinkImpl : IControlEventSink
{
    private readonly CohtmlView myView;

    public ViewEventSinkImpl(CohtmlView view)
    {
        myView = view;

        myView.Listener.ReadyForBindings += AttachBindings;
        if (myView.View.IsReadyForBindings())
            AttachBindings();
    }

    private void AttachBindings()
    {
        myView.View.RegisterForEvent("UixButtonClick", HandleButtonClick);
        myView.View.RegisterForEvent("UixToggleClick", HandleToggleClick);
        
        myView.View.ExecuteScript(GetInjectorJs());
    }
    
    private string GetInjectorJs()
    {
        using var sourceStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("UIExpansionKit.WebUi.Js.global-id-handler.js")!;
        using var targetStream = new MemoryStream();

        sourceStream.CopyTo(targetStream);

        return Encoding.UTF8.GetString(targetStream.ToArray());
    }

    private void HandleButtonClick(int globalId)
    {
        FeedbackSink?.ButtonClick(globalId);
    }

    private void HandleToggleClick(int globalId, bool isSelected)
    {
        FeedbackSink?.ToggleClick(globalId, isSelected);
    }

    public void SetVisible(int globalId, bool visible)
    {
        myView.View.TriggerEvent("UixControlSetVisible", globalId, visible);
    }

    public void SetText(int globalId, string text)
    {
        myView.View.TriggerEvent("UixControlSetText", globalId, text);
    }

    public void SetTextAnchor(int globalId, TextAnchor anchor)
    {
        // todo: switch here to ensure type safety
        myView.View.TriggerEvent("UixControlSetTestAnchor", globalId, anchor.ToString());
    }

    public void SetSelected(int globalId, bool selected)
    {
        myView.View.TriggerEvent("UixControlSetSelected", globalId, selected);
    }

    public IControlFeedbackSink? FeedbackSink { get; set; }
}