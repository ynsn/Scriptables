using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    public abstract class ScriptableObserverBehaviour : MonoBehaviour
    {
        [SerializeField] private bool enableDebug;

        [SerializeField] private bool disableOnNotifier;

        public bool DebugEnabled => enableDebug;

        public bool DisableOnNotifier => disableOnNotifier;
    }
}
