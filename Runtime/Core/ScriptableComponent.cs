using System;
using UnityEngine;

namespace StackMedia.Scriptables
{

    [Serializable]
    public class ScriptableComponent : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool foldout;

        [field: SerializeReference, HideInInspector] public ScriptableComponentContainer scriptable { get; internal set; }

        public T GetComponent<T>() where T : ScriptableComponent => scriptable.GetComponent<T>();
        public ScriptableComponent GetComponent(Type type) => scriptable.GetComponent(type);
    }
}
