using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a base class for components that observe scriptable events without a payload and respond via UnityEvents.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class implements the <see cref="IObserver"/> interface, allowing it to subscribe to and receive notifications from
    /// scriptable events that do not carry a value or payload. It is designed for use in Unity, enabling decoupled event-driven communication
    /// between ScriptableObjects and MonoBehaviour components.
    /// </para>
    /// <para>
    /// The observer subscribes to the specified <see cref="IObservable"/> event when enabled, and unsubscribes when disabled.
    /// When notified, it invokes the assigned <see cref="UnityEvent"/> response, allowing designers to configure reactions in the Unity Inspector.
    /// </para>
    /// <para>
    /// For observing scriptable events that broadcast notifications with a value or payload, see <see cref="ScriptableObserver{T}"/>.
    /// </para>
    /// </remarks>
    public abstract class ScriptableObserver : ScriptableObserverBehaviour, IObserver
    {
        /// <summary>
        /// The reference to the scriptable event to observe. Must implement <see cref="IObservable"/>.
        /// </summary>
        [SerializeField] private InterfaceReference<IObservable> scriptableEvent;

        /// <summary>
        /// The UnityEvent to invoke in response to the scriptable event notification.
        /// </summary>
        [SerializeField] private UnityEvent response = new UnityEvent();

        /// <summary>
        /// Subscribes this observer to the scriptable event when the component is enabled.
        /// </summary>
        protected virtual void OnEnable() => scriptableEvent?.Interface?.Subscribe(this);

        /// <summary>
        /// Unsubscribes this observer from the scriptable event when the component is disabled.
        /// </summary>
        protected virtual void OnDisable() => scriptableEvent?.Interface?.Unsubscribe(this);

        /// <summary>
        /// Called by the scriptable event to notify this observer that the event has occurred.
        /// Invokes the configured UnityEvent response and optionally disables the component.
        /// </summary>
        public void Notified()
        {
#if UNITY_EDITOR
            if (DebugEnabled) Debug.Log($"{gameObject.name} notified by {scriptableEvent.Object.name}", this);
#endif
            response.Invoke();
            if (DisableOnNotifier) enabled = false;
        }
    }
}
