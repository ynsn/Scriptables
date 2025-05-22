using System;
using UnityEngine;

namespace StackMedia.Scriptables
{

    [Serializable]
    public class ScriptableComponent : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool foldout;
        [field: SerializeReference, HideInInspector] public ScriptableComponentContainer scriptable { get; internal set; }
    }
}
