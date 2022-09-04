using System;
using UnityEngine;

#nullable enable

namespace UIExpansionKit.API.Controls
{
    public interface IMenuControl
    {
        /// <summary>
        /// Whether this control is visible or not.
        /// Invisible controls don't take up space in grid-like UI layouts
        /// </summary>
        public bool Visible { get; set; }
    }

    public interface IMenuControlWithText : IMenuControl
    {
        /// <summary>
        /// The current text shown on this control
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Text alignment for this control
        /// </summary>
        public TextAnchor Anchor { get; set; }
    }

    public interface IMenuButton : IMenuControlWithText
    {
    }

    public interface IMenuToggle : IMenuControlWithText
    {
        /// <summary>
        /// The state of the toggle. Manually setting it will fire set handler if the state changed.
        /// </summary>
        public bool Selected { get; set; }
    }

    public interface IMenuLabel : IMenuControlWithText
    {
    }
}