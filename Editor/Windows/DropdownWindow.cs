using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StackMedia.Scriptables.Editor
{
    public class DropdownWindow : EditorWindow
    {
        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            var visualAsset = Helpers.GetEditorVisualTreeAsset("Windows/DropdownWindow.uxml");
            visualAsset.CloneTree(root);

            var text = root.Q<Label>("text");
         
        }
    }
}
