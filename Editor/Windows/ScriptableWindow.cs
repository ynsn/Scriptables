using System;
using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    public class ScriptableWindow : EditorWindow
    {
        [MenuItem("Window/Scriptables/New Scriptable", priority = -150)]
        public static void OpenWindow()
        {
            var window = GetWindow<ScriptableWindow>();
            window.ShowModal();
        }
    }
}
