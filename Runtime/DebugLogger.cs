using UnityEngine;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a component-based wrapper for Unity's debug logging functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The DebugLogger component offers a convenient way to access logging functionality from
    /// UnityEvents, animation events, or other scenarios where direct code access isn't possible.
    /// It wraps Unity's Debug class methods in component methods that can be called from the Inspector.
    /// </para>
    /// <para>
    /// This class implements logging methods for both string messages and arbitrary objects, with
    /// variants for regular logs, warnings, and errors.
    /// </para>
    /// <para>
    /// Only one instance of this component can exist on a GameObject.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// This component can be referenced from UnityEvents:
    /// </para>
    /// <code>
    /// [SerializeField] private DebugLogger logger;
    /// [SerializeField] private Button button;
    /// 
    /// private void Start()
    /// {
    ///     button.onClick.AddListener(() => logger.Log("Button clicked!"));
    /// }
    /// </code>
    /// </example>
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
