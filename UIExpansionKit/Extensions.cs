using System.Reflection;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.UI;
using cohtml;
using cohtml.Net;
using UnityEngine;

namespace UIExpansionKit
{
    internal static class Extensions
    {
        private static FieldInfo? _internalCohtmlView = typeof(CohtmlControlledViewWrapper).GetField("_view", BindingFlags.Instance | BindingFlags.NonPublic);
        private static CohtmlControlledViewWrapper? _lastCohtmlView;
        private static View? _internalViewCache;

        public static void DestroyChildren(this Transform parent)
        {
            for (var i = parent.childCount; i > 0; i--) 
                Object.DestroyImmediate(parent.GetChild(i - 1).gameObject);
        }

        public static GameObject NoUnload(this GameObject obj)
        {
            obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return obj;
        }

        public static View? GetInternalView(this CohtmlControlledViewWrapper view)
        {
            if ((_internalViewCache == null || _lastCohtmlView != view) && _internalCohtmlView != null)
                _internalViewCache = (View)_internalCohtmlView.GetValue(view);

            _lastCohtmlView = view;

            return _internalViewCache;
        }
    }
}