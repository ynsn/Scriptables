using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Serves as the base class for all scriptable observer components, providing common functionality for debugging and behavior control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class forms the foundation of the scriptable observer hierarchy, defining common properties
    /// and behaviors that all observer implementations share. It extends MonoBehaviour to integrate with
    /// Unity's component system while providing specialized functionality for the Scriptables framework.
    /// </para>
    /// <para>
    /// ScriptableObserverBehaviour offers two key features:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Debug logging capabilities that can be toggled in the Inspector</description>
    ///   </item>
    ///   <item>
    ///     <description>The ability to automatically disable the observer component after it receives a notification</description>
    ///   </item>
    /// </list>
    /// <para>
    /// This class is not intended to be used directly but instead serves as a parent for specialized observer types like 
    /// <see cref="ScriptableObserver"/> and <see cref="ScriptableObserver{T}"/>, which implement concrete notification 
    /// handling for different event types.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class ScriptableObserverBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Controls whether debug logging is enabled for this observer.
        /// </summary>
        /// <remarks>
        /// When enabled, the observer will log notification events to the Unity console,
        /// which is useful for troubleshooting event flows during development.
        /// This setting is exposed in the Inspector for easy toggling.
        /// </remarks>
        [SerializeField] private bool enableDebug;

        /// <summary>
        /// Controls whether this observer should be disabled after receiving a notification.
        /// </summary>
        /// <remarks>
        /// When enabled, the observer component will automatically set its <see cref="Behaviour.enabled"/> property to false
        /// after processing a notification. This is useful for one-shot observers that should only respond to the first
        /// occurrence of an event. This setting is exposed in the Inspector for easy configuration.
        /// </remarks>
        [SerializeField] private bool disableOnNotifier;

        /// <summary>
        /// Gets whether debug logging is enabled for this observer.
        /// </summary>
        /// <value>
        /// <c>true</c> if debug logging is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool DebugEnabled => enableDebug;

        /// <summary>
        /// Gets whether this observer should be disabled after receiving a notification.
        /// </summary>
        /// <value>
        /// <c>true</c> if the observer should disable itself after notification; otherwise, <c>false</c>.
        /// </value>
        public bool DisableOnNotifier => disableOnNotifier;
    }
}
