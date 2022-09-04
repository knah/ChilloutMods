using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using cohtml;
using MelonLoader.ICSharpCode.SharpZipLib.Core;

namespace UIExpansionKit;

public class CohtmlDataPatch
{
    private const string UrlPrefix = "uix-resource:";
    
    internal static bool Prefix(ref IEnumerator __result, DefaultResourceHandler.ResourceRequestData __0)
    {
        var uri = __0.UriBuilder.ToString();
        if (!uri.StartsWith(UrlPrefix)) return true;
        
        __result = Empty.EmptyArray<object>.Value.GetEnumerator();

        uri = uri.Substring(UrlPrefix.Length);
        
        FruityLogger.Msg($"Got data request! {(uri.Length > 100 ? uri.Substring(0, 100) : uri)}");

        var data = GetResourceBytes(uri);
        if (data == null)
        {
            __0.Response.SetStatus(404);
            __0.RespondWithFailure("Not found");
            return false;
        }

        var space = __0.Response.GetSpace((ulong) data.Length);
        Marshal.Copy(data, 0, space, data.Length);
        __0.Error = "";
        __0.RespondWithSuccess();
        
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