namespace UIExpansionKit.WebUi;

internal static class GlobalControlId
{
    private static int ourGlobalControlId;

    internal static int Next()
    {
        return ourGlobalControlId++;
    }
}