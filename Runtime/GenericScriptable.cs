using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GenericOrAbstractTypeAttribute : Attribute
    {
    }

    [Serializable]
    public class GenericScriptable : ScriptableObject
    {
    }
}
