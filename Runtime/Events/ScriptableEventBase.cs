using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    public abstract class ScriptableEventBase : ScriptableObject
    {
        [SerializeField] protected string comment;
        
        public string Comment => comment;
    }
}
