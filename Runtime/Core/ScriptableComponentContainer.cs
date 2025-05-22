using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackMedia.Scriptables
{
    [Serializable]
    public class ScriptableComponentContainer : ScriptableObject, IComponentContainer
    {
        [SerializeField] private List<ScriptableComponent> components = new List<ScriptableComponent>();

        public IReadOnlyList<ScriptableComponent> Components => components;

        internal void Clear() => components.Clear();

        public T AddComponent<T>() where T : ScriptableComponent
        {
            var instance = CreateInstance<T>();
            instance.name = typeof(T).Name;
            _ = TryAddSubAsset(instance);
            components.Add(instance);
            instance.scriptable = this as Scriptable;
            return instance;
        }

        public ScriptableComponent AddComponent(Type type)
        {
            if (type == null || !typeof(ScriptableComponent).IsAssignableFrom(type)) return null;

            if (CreateInstance(type) is not ScriptableComponent instance) return null;

            instance!.name = type.Name;
            _ = TryAddSubAsset(instance);
            components.Add(instance);
            instance.scriptable = this as Scriptable;
            return instance;
        }

        public T GetComponent<T>() where T : ScriptableComponent
        {
            foreach (ScriptableComponent scriptableComponent in components)
            {
                if (scriptableComponent is T typedComponent)
                    return typedComponent;

            }
            return null;
        }

        public void DestroyComponentAt(int index)
        {
            if (index < 0 || index >= components.Count) return;

            ScriptableComponent component = components[index];
#if UNITY_EDITOR
            AssetDatabase.RemoveObjectFromAsset(components[index]);
#endif
            DestroyImmediate(component, true);
            components.RemoveAt(index);
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public ScriptableComponent GetComponent(Type type)
        {
            if (!typeof(ScriptableComponent).IsAssignableFrom(type)) return null;

            var count = components.Count;
            for (var i = 0; i < count; i++)
            {
                ScriptableComponent component = components[i];
                if (type.IsAssignableFrom(component.GetType())) return component;
            }
            return null;
        }
        public T GetComponentAtIndex<T>(int index) where T : ScriptableComponent
        {
            if (index < 0 || index >= components.Count) return null;
            ScriptableComponent component = components[index];
            if (component is T typedComponent) return typedComponent;
            return null;
        }
        
        public ScriptableComponent GetComponentAtIndex(int index) => throw new NotImplementedException();
        public int GetComponentCount() => throw new NotImplementedException();
        public int GetComponentIndex<T>(T component) where T : ScriptableComponent => throw new NotImplementedException();
        public int GetComponentIndex(ScriptableComponent component) => throw new NotImplementedException();
        public T[] GetComponents<T>() where T : ScriptableComponent => throw new NotImplementedException();
        public void GetComponents<T>(IList<T> results) where T : ScriptableComponent
        {
            throw new NotImplementedException();
        }
        public ScriptableComponent[] GetComponents(Type type) => throw new NotImplementedException();
        public void GetComponents(Type type, IList<ScriptableComponent> results)
        {
            throw new NotImplementedException();
        }

#if UNITY_EDITOR
        private bool TryAddSubAsset(ScriptableComponent component)
        {
            AssetDatabase.AddObjectToAsset(component, this);
            if (Application.isPlaying) return true;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return true;
        }
#else
        private bool TryAddSubAsset(ScriptableComponent component)
        {
            return false;
        }
#endif

    }
}
