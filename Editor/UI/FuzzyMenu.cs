using System;
using System.Collections.Generic;
using System.Linq;
using StackMedia.Scriptables.Editor.UI.Internal;
using UnityEngine;

namespace StackMedia.Scriptables.Editor.UI
{
    public class FuzzyMenuItem<T>
    {
        public string Name { get; private set; }

        public Texture Icon { get; set; }

        public T UserData { get; set; }

        public List<FuzzyMenuItem<T>> Children { get; } = new List<FuzzyMenuItem<T>>();

        public FuzzyMenuItem(string name)
        {
            Name = name;
        }

        public FuzzyMenuItem<T> AddChild(FuzzyMenuItem<T> child)
        {
            Children.Add(child);
            return this;
        }

        public FuzzyMenuItem<T> AddChild(string name)
        {
            var child = new FuzzyMenuItem<T>(name);
            Children.Add(child);
            return child;
        }
    }

    public class FuzzyMenu<T>
    {
        private FuzzyMenuController<T> controller;
        private FuzzyMenuWindow window;

        public FuzzyMenuItem<T> RootItem { get; }

        public Action<T> OnItemSelected { get; set; }

        public FuzzyMenu(string rootName)
        {
            RootItem = new FuzzyMenuItem<T>(rootName);
        }

        public FuzzyMenu(FuzzyMenuItem<T> rootItem) => RootItem = rootItem;


        public void Show(Rect position)
        {
            if (window == null) window = ScriptableObject.CreateInstance<FuzzyMenuWindow>();

            controller ??= new FuzzyMenuController<T>(RootItem);

            controller.ItemSelected = OnItemSelected;
            window.OnEditorCreated += controller.OnEditorWindowCreated;

            Vector2 size = position.size;
            size.y += 400;
            window.ShowAsDropDown(position, size);
        }
    }
}
