public static class SceneTransitionContext
{
    public static string EntryPointId { get; set; }

    public static bool SpawnedFromEntryPoint { get; set; }

    public static bool HasPendingEntryPoint =>
        !string.IsNullOrEmpty(EntryPointId);

    public static void ClearEntryPoint()
    {
        EntryPointId = null;
    }

    public static void ClearAll()
    {
        EntryPointId = null;
        SpawnedFromEntryPoint = false;
    }
}