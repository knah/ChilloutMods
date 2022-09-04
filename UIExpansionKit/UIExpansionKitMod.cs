using System;
using System.Collections;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using HarmonyLib;
using MelonLoader;
using UIExpansionKit;
using UIExpansionKit.API;
using UIExpansionKit.WebUi.Events;
using UnityEngine;
using UnityEngine.Rendering;

[assembly:MelonInfo(typeof(UiExpansionKitMod), "UI Expansion Kit", "1.0.0", "knah")]
[assembly:MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace UIExpansionKit
{
    public class UiExpansionKitMod : MelonMod
    {
        private static UiExpansionKitMod? ourInstance;
        private HtmlModSettingsHandler? myModSettingsHandler;
        private ViewEventSinkImpl? myMainMenuEventSink;
        
        public override void OnApplicationStart()
        {
            ExpansionKitSettings.RegisterSettings();

            ourInstance = this;

            HarmonyInstance.Patch(
                AccessTools.Method(typeof(ViewManager), nameof(ViewManager.UiStateToggle), new[] { typeof(bool) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(UiExpansionKitMod), nameof(UiStateToggleSuffix))));

            HarmonyInstance.Patch(
                AccessTools.Method(typeof(DefaultResourceHandler), nameof(DefaultResourceHandler.RequestResourceAsync)),
                new HarmonyMethod(typeof(CohtmlDataPatch), nameof(CohtmlDataPatch.Prefix)));

            MelonCoroutines.Start(InitThings());
        }

        public static void UiStateToggleSuffix(ViewManager __instance, bool __0)
        {
            if (__0)
                ourInstance?.CheckMenu();

            if (ExpansionKitSettings.MainMenuInstant.Value)
            {
                var animator = __instance.uiMenuAnimator;
                // it needs two updates for some odd reason
                animator.Update(10f);
                animator.Update(10f);
            }
        }

        private void CheckMenu()
        {
            if (myModSettingsHandler != null) return;
            
            myModSettingsHandler = new HtmlModSettingsHandler();
            myModSettingsHandler.PerformInjection(ViewManager.Instance.gameMenuView);
        }

        private IEnumerator InitThings()
        {
            while (ViewManager.Instance == null || ViewManager.Instance.gameMenuView == null)
                yield return null;

            myMainMenuEventSink = new ViewEventSinkImpl(ViewManager.Instance.gameMenuView);
            foreach (var page in ExpansionKitApi.SettingPageExtensions.Values) 
                page.Sink = myMainMenuEventSink;

            FruityLogger.Msg("Init done!");
        }
    }
}