using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    public class Scriptable : ScriptableComponentContainer, IScriptable
    {
        [SerializeField] internal string comment;

        public virtual SerializedType[] Types { get; } = Array.Empty<SerializedType>();

        public string Comment => comment;
    }
}
