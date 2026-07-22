public static class SceneTransitionContext
{
    public static string EntryPointId { get; set; }

    public static bool SpawnedFromEntryPoint { get; set; }

    public static bool UseEntryPointOnLoad
    {
        get
        {
            return !string.IsNullOrEmpty(EntryPointId) || SpawnedFromEntryPoint;
        }
    }

    public static void Clear()
    {
        EntryPointId = null;
        SpawnedFromEntryPoint = false;
    }

    public static void ClearEntryPoint()
    {
        Clear();
    }
}