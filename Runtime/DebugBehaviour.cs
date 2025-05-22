using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    public class DebugBehaviour : ScriptableBehaviour
    {
        [SerializeField] private bool logEnabled = true;

        [SerializeField] private bool logWarningEnabled;

        [SerializeField] private bool logErrorEnabled;
    }
}
