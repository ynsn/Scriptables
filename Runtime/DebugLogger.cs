using UnityEngine;

namespace StackMedia.Scriptables
{
    [AddComponentMenu("Scriptables/Debug/Logger")]
    [DisallowMultipleComponent]
    public sealed class DebugLogger : MonoBehaviour
    {
        public void Log(string message) => Debug.Log(message);
        
        public void Log(object obj) => Debug.Log(obj);
        
        public void LogWarning(string message) => Debug.LogWarning(message);
        
        public void LogWarning(object obj) => Debug.LogWarning(obj);
        
        public void LogError(string message) => Debug.LogError(message);
        
        public void LogError(object obj) => Debug.LogError(obj);
    }
}
