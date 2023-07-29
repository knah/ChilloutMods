using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using HarmonyLib;
using MelonLoader;
using MelonLoader.ICSharpCode.SharpZipLib.Core;
using UIExpansionKit;
using UIExpansionKit.API;
using UIExpansionKit.WebUi.Events;
using UnityEngine;
using UnityEngine.Rendering;

[assembly:MelonInfo(typeof(UiExpansionKitMod), "UI Expansion Kit", "1.1.1", "knah & DDAkebono")]
[assembly:MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace UIExpansionKit
{
    public class UiExpansionKitMod : MelonMod
    {
        private static UiExpansionKitMod? ourInstance;
        private HtmlModSettingsHandler? myModSettingsHandler;
        private ViewEventSinkImpl? myMainMenuEventSink;
        
        public override void OnInitializeMelon()
        {
            ExpansionKitSettings.RegisterSettings();

            ourInstance = this;

            HarmonyInstance.Patch(
                AccessTools.Method(typeof(ViewManager), nameof(ViewManager.UiStateToggle), new[] { typeof(bool) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(UiExpansionKitMod), nameof(UiStateToggleSuffix))));
            
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

    [HarmonyPatch(typeof(DefaultResourceHandler))]
    class CohtmlDataPatch
    {
        private const string UrlPrefix = "uix-resource:";
        
        [HarmonyPatch(nameof(DefaultResourceHandler.RequestResourceAsync))]
        [HarmonyPrefix]
        static bool RequestResAsync(DefaultResourceHandler.ResourceRequestData requestData, ref IEnumerator __result)
        {
            var uri = requestData.UriBuilder.ToString();
            if (!uri.StartsWith(UrlPrefix)) return true;

            __result = Empty.EmptyArray<int>.Value.GetEnumerator();

            uri = uri.Substring(UrlPrefix.Length);

            FruityLogger.Msg($"Got data request! {(uri.Length > 100 ? uri.Substring(0, 100) : uri)}");

            var data = GetResourceBytes(uri);
            if (data == null)
            {
                requestData.Response.SetStatus(404);
                requestData.RespondWithFailure("Not found");
                return false;
            }

            var space = requestData.Response.GetSpace((ulong)data.Length);
            Marshal.Copy(data, 0, space, data.Length);
            requestData.Error = "";
            requestData.RespondWithSuccess();

            return false;
        }
        
        private static byte[]? GetResourceBytes(string resourcePath)
        {
            using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"UIExpansionKit.WebUi.Js.{resourcePath}");
            if (resourceStream == null)
                return null;

            using var memStream = new MemoryStream();
            resourceStream.CopyTo(memStream);

            return memStream.ToArray();
        }
    }
}