using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    public class Scriptable : ScriptableObject, IScriptable
    {
        [SerializeField] private string comment;

        [SerializeField] private bool debugEnabled;

        public virtual SerializedType[] Types { get; } = Array.Empty<SerializedType>();

        public string Comment => comment;

        public bool DebugEnabled => debugEnabled;
    }
}
