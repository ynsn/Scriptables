using System;
using System.Collections.Generic;

namespace StackMedia.Scriptables
{
    public interface IComponentContainer
    {
        T AddComponent<T>() where T : ScriptableComponent;

        ScriptableComponent AddComponent(Type type);

        T GetComponent<T>() where T : ScriptableComponent;

        ScriptableComponent GetComponent(Type type);

        T GetComponentAtIndex<T>(int index) where T : ScriptableComponent;

        ScriptableComponent GetComponentAtIndex(int index);

        int GetComponentCount();

        int GetComponentIndex<T>(T component) where T : ScriptableComponent;

        int GetComponentIndex(ScriptableComponent component);

        T[] GetComponents<T>() where T : ScriptableComponent;

        void GetComponents<T>(IList<T> results) where T : ScriptableComponent;

        ScriptableComponent[] GetComponents(Type type);

        void GetComponents(Type type, IList<ScriptableComponent> results);
    }
}
