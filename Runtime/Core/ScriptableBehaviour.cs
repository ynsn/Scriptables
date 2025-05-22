using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    public class ScriptableBehaviour : ScriptableComponent, IBehaviour
    {
        [SerializeField, HideInInspector] private bool enabled = true;
    }
}
