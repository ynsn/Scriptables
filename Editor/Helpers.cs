using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StackMedia.Scriptables.Editor
{
    public static class Helpers
    {
        public static VisualTreeAsset GetEditorVisualTreeAsset(string name)
        {
            var assetName = name.EndsWith(".uxml") ? name : $"{name}.uxml";
            var path = $"{Constants.Paths.PackageEditor}{assetName}";
            var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            if (treeAsset != null) return treeAsset;
            Debug.LogError($"Could not find VisualTreeAsset at path: {path}");
            return null;
        }

        public static StyleSheet GetStyleSheet(string name)
        {
            var assetName = name.EndsWith(".uss") ? name : $"{name}.uss";
            var path = $"{Constants.Paths.PackageEditor}{assetName}";
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            if (styleSheet != null) return styleSheet;
            Debug.LogError($"Could not find StyleSheet at path: {path}");
            return null;
        }
        
        public static void RepaintActiveEditors()
        {
            foreach (UnityEditor.Editor editor in ActiveEditorTracker.sharedTracker.activeEditors) editor.Repaint();
        }
    }
}
