using System.Collections.Generic;
using StackMedia.Scriptables.Editor.UI.Internal;
using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor.UI
{
    public struct SearchMenuItem<T>
    {
        public string Name { get; private set; }

        public Texture Icon { get; private set; }

        public T UserData { get; private set; }

        public List<SearchMenuItem<T>> Children { get; private set; }

        public SearchMenuItem(string name, Texture icon, T userData)
        {
            Name = name;
            Icon = icon;
            UserData = userData;
            Children = null;
        }

        public SearchMenuItem(string name, Texture icon, List<SearchMenuItem<T>> children)
        {
            Name = name;
            Icon = icon;
            UserData = default;
            Children = children;
        }
    }

    public class SearchMenu<T>
    {
        private SearchMenuEditorWindow window;

        public SearchMenuItem<T> Root { get; }

        public SearchMenu(string rootName = "Root")
        {
            window = ScriptableObject.CreateInstance<SearchMenuEditorWindow>();

            Root = new SearchMenuItem<T>(rootName, null, null);
        }

        public void ShowMenu(Rect position)
        {
            window ??= ScriptableObject.CreateInstance<SearchMenuEditorWindow>();

            Vector2 size = position.size;
            size.y += 400;
            window.ShowAsDropDown(position, size);
        }
    }
}
