using System.Reflection;
using MelonLoader;

internal static class FruityLogger
{
    private static readonly MelonLogger.Instance Instance = new MelonLogger.Instance(Assembly.GetExecutingAssembly().GetCustomAttribute<MelonInfoAttribute>().Name);

    public static void Msg(string str) => Instance.Msg(str);
    public static void Error(string str) => Instance.Error(str);
    public static void Warning(string str) => Instance.Warning(str);
}