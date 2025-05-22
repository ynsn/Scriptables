using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a base class for components that observe scriptable events with a typed payload and respond via UnityEvents.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract generic class implements the <see cref="IObserver{T}"/> interface, allowing it to subscribe to and receive notifications
    /// from scriptable events that carry a value or payload of type <typeparamref name="T"/>. It is designed for use in Unity, enabling
    /// decoupled event-driven communication between ScriptableObjects and MonoBehaviour components with type safety.
    /// </para>
    /// <para>
    /// The observer subscribes to the specified <see cref="IObservable{T}"/> event when enabled, and unsubscribes when disabled.
    /// When notified with a payload, it invokes the assigned <see cref="UnityEvent{T}"/> response with that payload, allowing designers
    /// to configure type-safe reactions in the Unity Inspector.
    /// </para>
    /// <para>
    /// For observing scriptable events that broadcast notifications without a value or payload, see <see cref="ScriptableObserver"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the payload that this observer can receive and process.</typeparam>
    public abstract class ScriptableObserver<T> : ScriptableObserverBehaviour, IObserver<T>
    {
        /// <summary>
        /// The reference to the scriptable event to observe. Must implement <see cref="IObservable{T}"/> with the same type parameter.
        /// </summary>
        [SerializeField] private InterfaceReference<IObservable<T>> scriptableEvent;

        /// <summary>
        /// The UnityEvent to invoke in response to the scriptable event notification, passing the received payload as an argument.
        /// </summary>
        [SerializeField] private UnityEvent<T> response = new UnityEvent<T>();

        /// <summary>
        /// Subscribes this observer to the scriptable event when the component is enabled.
        /// </summary>
        protected virtual void OnEnable() => scriptableEvent?.Interface?.Subscribe(this);

        /// <summary>
        /// Unsubscribes this observer from the scriptable event when the component is disabled.
        /// </summary>
        protected virtual void OnDisable() => scriptableEvent?.Interface?.Unsubscribe(this);

        /// <summary>
        /// Called by the scriptable event to notify this observer that the event has occurred, providing the event payload.
        /// Invokes the configured UnityEvent response with the received payload and optionally disables the component.
        /// </summary>
        /// <param name="arg">The payload or data associated with the event notification.</param>
        public void Notified(T arg)
        {
#if UNITY_EDITOR
            if (DebugEnabled)
            {
                Debug.Log("Invoked", this);
            }
#endif
            response.Invoke(arg);
            if (DisableOnNotifier) enabled = false;
        }
    }
}
