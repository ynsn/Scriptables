using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a base class for components that observe non-typed scriptable events and implement the observer pattern.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class implements the <see cref="IObserver"/> interface, enabling it to subscribe to and receive notifications
    /// from scriptable events that don't carry any payload. It follows Unity's component lifecycle, automatically
    /// subscribing to the observable when enabled and unsubscribing when disabled.
    /// </para>
    /// <para>
    /// ObserverBehaviour serves as a foundation for creating decoupled event-driven architectures in Unity, where
    /// MonoBehaviour components need to react to events broadcast by ScriptableObjects without direct references.
    /// The subscription mechanism is handled automatically, simplifying the implementation of event listeners.
    /// </para>
    /// <para>
    /// The <see cref="Notified"/> method provides a virtual implementation that derived classes can override to
    /// implement specific behavior when the observed event is triggered. This design pattern promotes separation
    /// of concerns and facilitates modular, reusable components.
    /// </para>
    /// <para>
    /// For observers that need to receive typed payloads with their notifications, see <see cref="ObserverBehaviour{T}"/>.
    /// </para>
    /// </remarks>
    [Serializable]
    public class ObserverBehaviour : MonoBehaviour, IObserver
    {
        /// <summary>
        /// The reference to the observable event that this component will listen to.
        /// </summary>
        /// <remarks>
        /// This field is serialized and exposed in the inspector, allowing designers to wire up
        /// event connections without coding. The <see cref="InterfaceReference{T}"/> wrapper enables
        /// Unity to serialize the interface reference, which it otherwise couldn't handle directly.
        /// </remarks>
        [SerializeField] private InterfaceReference<IObservable> observable;

        /// <summary>
        /// Subscribes this observer to the configured observable event when the component is enabled.
        /// </summary>
        /// <remarks>
        /// This method is called automatically by Unity when the component becomes active. It registers
        /// this observer with the observable using null-conditional operators to handle cases where the
        /// observable reference might not be set.
        /// </remarks>
        protected virtual void OnEnable() => observable?.Interface?.Subscribe(this);

        /// <summary>
        /// Unsubscribes this observer from the configured observable event when the component is disabled.
        /// </summary>
        /// <remarks>
        /// This method is called automatically by Unity when the component becomes inactive. It removes
        /// this observer from the observable's notification list using null-conditional operators to handle
        /// cases where the observable reference might not be set.
        /// </remarks>
        protected virtual void OnDisable() => observable?.Interface?.Unsubscribe(this);

        /// <summary>
        /// Called by the observable to notify this observer that the event has occurred.
        /// </summary>
        /// <remarks>
        /// This virtual method is invoked when the observed event is triggered. By default, it does nothing,
        /// but derived classes can override it to implement specific reactions to the event. This method
        /// forms the core of the observer pattern, allowing objects to respond to events without direct coupling.
        /// </remarks>
        public virtual void Notified() { }
    }
}
