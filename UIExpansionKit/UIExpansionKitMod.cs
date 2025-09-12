using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using HarmonyLib;
using MelonLoader;
using UIExpansionKit;
using UIExpansionKit.API;
using UIExpansionKit.WebUi.Events;

[assembly:MelonInfo(typeof(UiExpansionKitMod), "UI Expansion Kit", "1.1.8", "knah & DDAkebono")]
[assembly:MelonGame("ChilloutVR", "ChilloutVR")]

namespace UIExpansionKit
{
    public class UiExpansionKitMod : MelonMod
    {
        private static UiExpansionKitMod? ourInstance;
        private HtmlModSettingsHandler? myModSettingsHandler;
        internal static ViewEventSinkImpl? MyMainMenuEventSink;
        
        public override void OnInitializeMelon()
        {
            ourInstance = this;

            HarmonyInstance.Patch(
                AccessTools.Method(typeof(ViewManager), nameof(ViewManager.UiStateToggle), new[] { typeof(bool) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(UiExpansionKitMod), nameof(UiStateToggleSuffix))));
        }

        public static void UiStateToggleSuffix(ViewManager __instance, bool __0)
        {
            if (__0)
                ourInstance?.CheckMenu();
        }

        private void CheckMenu()
        {
            if (myModSettingsHandler != null) return;
            
            myModSettingsHandler = new HtmlModSettingsHandler();
            myModSettingsHandler.PerformInjection(ViewManager.Instance.cohtmlView);
        }
    }

    [HarmonyPatch(typeof(ViewManager))]
    class ViewManagerPatch
    {
        [HarmonyPatch(nameof(ViewManager.OnFinishedLoad))]
        [HarmonyPostfix]
        static void OnViewManagerStart()
        {
            UiExpansionKitMod.MyMainMenuEventSink = new ViewEventSinkImpl(ViewManager.Instance.cohtmlView);
            foreach (var page in ExpansionKitApi.SettingPageExtensions.Values)
                page.Sink = UiExpansionKitMod.MyMainMenuEventSink;

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

	internal static class Empty
	{
		internal static class EmptyArray<T>
		{
			public static readonly T[] Value = new T[0];
		}
		public static T[] Array<T>() => EmptyArray<T>.Value;
	}
}