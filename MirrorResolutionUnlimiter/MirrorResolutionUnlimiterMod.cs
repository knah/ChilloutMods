using System.ComponentModel;
using System.Linq;
using MelonLoader;
using MirrorResolutionUnlimiter;
using UnityEngine;

[assembly:MelonInfo(typeof(MirrorResolutionUnlimiterMod), "MirrorResolutionUnlimiter", "1.0.0", "knah", "https://github.com/knah/ChilloutMods")]
[assembly:MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly:MelonOptionalDependencies("UIExpansionKit")]

namespace MirrorResolutionUnlimiter
{
    internal class MirrorResolutionUnlimiterMod : MelonMod
    {
        public const string SettingsCategory = "MirrorResolutionUnlimiter";
        
        private static bool ourAllMirrorsAuto = false;
        internal static MelonPreferences_Entry<bool> UiInMirrors;

        private MelonPreferences_Entry<PixelLightMode> myPixelLightsSetting;

        public override void OnApplicationStart()
        {

            var category = MelonPreferences.CreateCategory(SettingsCategory, "Mirror Resolution");
            var forceAutoRes = category.CreateEntry("AllMirrorsUseAutoRes", false, "Force auto resolution");
            forceAutoRes.OnValueChanged += (_, v) =>
            {
                ourAllMirrorsAuto = v;
                UpdateMirrorParams();
            };
            ourAllMirrorsAuto = forceAutoRes.Value;
            
            myPixelLightsSetting = category.CreateEntry("PixelLights", PixelLightMode.Default, "Pixel lights in mirrors");
            myPixelLightsSetting.OnValueChangedUntyped += UpdateMirrorParams;

            UiInMirrors = category.CreateEntry("UiInMirrors", false, "Include UI in mirrors when using Optimize/Beautify buttons");

            if (MelonHandler.Mods.Any(it => it.Info.Name == "UI Expansion Kit"))
            {
                MelonLogger.Msg("Adding UIExpansionKit buttons");
                UiExtensionsAddon.Init();
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (buildIndex != -1) return;
            
            foreach (var mirror in Resources.FindObjectsOfTypeAll<CVRMirror>())
            {
                var store = mirror.gameObject.GetComponent<OriginalMirrorSettingKeeper>() ??
                            mirror.gameObject.AddComponent<OriginalMirrorSettingKeeper>();
                store.OriginalPixelLights = mirror.m_DisablePixelLights;
                store.OriginalTextureRes = mirror.m_TextureSize;
            }

            UpdateMirrorParams();
        }

        private void UpdateMirrorParams()
        {
            var allMirrors = Resources.FindObjectsOfTypeAll<CVRMirror>();
            var pixelLightMode = myPixelLightsSetting.Value;
            foreach (var mirror in allMirrors)
            {
                mirror.m_DisablePixelLights = pixelLightMode switch
                {
                    PixelLightMode.Default => mirror.gameObject.GetComponent<OriginalMirrorSettingKeeper>()?.OriginalPixelLights ?? mirror.m_DisablePixelLights,
                    PixelLightMode.ForceDisable => true,
                    PixelLightMode.ForceAllow => false,
                    _ => mirror.m_DisablePixelLights
                };
                mirror.m_TextureSize = ourAllMirrorsAuto ? 65536
                    : mirror.gameObject.GetComponent<OriginalMirrorSettingKeeper>()?.OriginalTextureRes ?? mirror.m_TextureSize;
                mirror.updateRenderResolution();
            }
        }

        enum PixelLightMode
        {
            Default,
            [Description("Force allow")]
            ForceAllow,
            [Description("Force disable")]
            ForceDisable
        }
    }
}