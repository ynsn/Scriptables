using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    public class ScriptableEventListenerBase : MonoBehaviour
    {
        [SerializeField] protected float responseDelay;

        [SerializeField] protected bool enableDebug;

        /// <summary>
        /// Gets or sets the delay in seconds before the response is invoked.
        /// </summary>
        public float ResponseDelay { get => responseDelay; set => responseDelay = value; }

        /// <summary>
        /// Gets whether the listener logs debug information.
        /// </summary>
        public bool IsDebugEnabled => enableDebug;
    }
}
