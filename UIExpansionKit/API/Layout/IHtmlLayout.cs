namespace UIExpansionKit.API.Layout;

public interface IHtmlLayout
{
    /// <summary>
    /// Clears this layout, removing all controls from it
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Produces the final HTML with all controls
    /// </summary>
    string GetHtml();
}

public interface IHtmlLayout<TLayoutParameter> : IHtmlLayout where TLayoutParameter : struct
{
    /// <summary>
    /// Append a control to this layout's grid
    /// </summary>
    /// <param name="controlHtmlString">the control's HTML string</param>
    /// <param name="parameter">Optional parameter to control how this component is laid out</param>
    void AddControl(string controlHtmlString, TLayoutParameter? parameter);
}