namespace StackMedia.Scriptables.Editor
{
    public static class Constants
    {
        public static class Paths
        {
            public const string PackageRoot = "Packages/nl.stackmedia.scriptables/";
            public const string PackageEditor = PackageRoot + "Editor/";
            public const string PackageScriptTemplates = PackageEditor + "Templates/";
            
            public const string PackageRuntime = PackageRoot + "Runtime/";
        }
        
        public static class MenuItems
        {
            public const string WindowRoot = "Window/Scriptables/";
            public const int WindowRootPriority = 2025;
            
            public const string TypesEditorWindow = WindowRoot + "Types";
            public const int TypesEditorWindowPriority = WindowRootPriority + 1;
        }
    }
}
