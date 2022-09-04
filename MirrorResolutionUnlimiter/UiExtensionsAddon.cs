using System.Runtime.CompilerServices;
using UIExpansionKit.API;
using UnityEngine;

namespace MirrorResolutionUnlimiter
{
    public static class UiExtensionsAddon
    {
        private static readonly int PlayerLayers = LayerMask.GetMask("PlayerNetwork", "PlayerLocal");
        private static readonly int UiLayerMask = LayerMask.GetMask("UI");
        private static readonly int MirrorReflectionLayer = LayerMask.GetMask("MirrorReflection");
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Init()
        {
            var settings = ExpansionKitApi.GetSettingsCategory(MirrorResolutionUnlimiterMod.SettingsCategory);
            settings.AddSimpleButton("Optimize mirrors", OptimizeMirrors);
            settings.AddSimpleButton("Beautify mirrors", BeautifyMirrors);
        }

        private static void BeautifyMirrors()
        {
            foreach (var mirror in Object.FindObjectsOfType<CVRMirror>())
                if (mirror.isActiveAndEnabled)
                    if (MirrorResolutionUnlimiterMod.UiInMirrors.Value)
                        mirror.m_ReflectLayers = -1;
                    else
                        mirror.m_ReflectLayers =
                            -1 & ~UiLayerMask;

        }

        private static void OptimizeMirrors()
        {
            foreach (var mirror in Object.FindObjectsOfType<CVRMirror>())
                if (mirror.isActiveAndEnabled)
                    mirror.m_ReflectLayers = PlayerLayers | MirrorReflectionLayer | (MirrorResolutionUnlimiterMod.UiInMirrors.Value ? UiLayerMask : 0);
        }
    }
}