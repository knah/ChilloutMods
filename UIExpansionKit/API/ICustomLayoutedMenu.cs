using System;
using UIExpansionKit.API.Controls;

namespace UIExpansionKit.API
{
    public interface ICustomLayoutedMenu<TLayoutParam> where TLayoutParam: struct
    {
        /// <summary>
        /// Adds a simple button to this custom menu
        /// </summary>
        /// <param name="text">User-visible button text</param>
        /// <param name="onClick">Button click action</param>
        /// <param name="layoutParam">Optional parameter to affect how this component is laid out</param>
        IMenuButton AddSimpleButton(string text, Action onClick, TLayoutParam? layoutParam = null);
        
        /// <summary>
        /// Adds a simple button to this custom menu
        /// </summary>
        /// <param name="text">User-visible button text, receives the pressed button as parameter</param>
        /// <param name="onClick">Button click action</param>
        /// <param name="layoutParam">Optional parameter to affect how this component is laid out</param>
        IMenuButton AddSimpleButton(string text, Action<IMenuButton> onClick, TLayoutParam? layoutParam = null);

        /// <summary>
        /// Adds a toggle button to this custom menu
        /// </summary>
        /// <param name="text">User-visible button text</param>
        /// <param name="onClick">This action will be called when button state is toggled</param>
        /// <param name="getInitialState">(optional) this func will be called to get the initial state of this button. If will default to not-set if this is not provided.</param>
        /// <param name="instanceConsumer">(optional) this action will be invoked when the button is instantiated</param>
        /// <param name="layoutParam">Optional parameter to affect how this component is laid out</param>
        IMenuToggle AddToggleButton(string text, Action<bool> onClick, Func<bool>? getInitialState = null, TLayoutParam? layoutParam = null);

        /// <summary>
        /// Adds a label to custom menu
        /// </summary>
        /// <param name="text">User-visible text</param>
        /// <param name="layoutParam">Optional parameter to affect how this component is laid out</param>
        IMenuLabel AddLabel(string text, TLayoutParam? layoutParam = null);

        /// <summary>
        /// Adds an empty spot in menu layout.
        /// </summary>
        /// <param name="layoutParam">Optional parameter to affect how this component is laid out</param>
        IMenuControl AddSpacer(TLayoutParam? layoutParam = null);
    }
}