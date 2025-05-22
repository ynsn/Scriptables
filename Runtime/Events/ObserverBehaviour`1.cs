using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a base class for components that observe scriptable events with a typed payload.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This generic class implements the <see cref="IObserver{T}"/> interface, allowing it to subscribe to and receive notifications
    /// from observable sources that emit events with a payload of type <typeparamref name="T"/>. It integrates with Unity's component
    /// lifecycle to automatically manage subscriptions.
    /// </para>
    /// <para>
    /// ObserverBehaviour&lt;T&gt; serves as a foundation for creating decoupled event-driven architectures in Unity, where
    /// components need to react to events with typed data without direct references to the event sources. The subscription
    /// mechanism is handled automatically during component enable/disable cycles.
    /// </para>
    /// <para>
    /// The <see cref="Notified(T)"/> method provides a virtual implementation that derived classes can override to
    /// implement specific behavior when the observed event is triggered with a payload. This design pattern promotes
    /// separation of concerns and facilitates type-safe event handling.
    /// </para>
    /// <para>
    /// For specialized implementations that use Unity's event system to expose reactions in the inspector, see 
    /// <see cref="ScriptableObserver{T}"/>. For observers that don't require a typed payload, see <see cref="ObserverBehaviour"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the payload that this observer can receive and process.</typeparam>
    [Serializable]
    public class ObserverBehaviour<T> : MonoBehaviour, IObserver<T>
    {
        /// <summary>
        /// The reference to the observable event that this component will listen to.
        /// </summary>
        /// <remarks>
        /// This field is serialized and exposed in the inspector, allowing designers to wire up
        /// event connections without coding. The <see cref="InterfaceReference{T}"/> wrapper enables
        /// Unity to serialize references to objects implementing the <see cref="IObservable{T}"/> interface,
        /// which Unity otherwise couldn't handle directly.
        /// </remarks>
        [SerializeField] private InterfaceReference<IObservable<T>> observable;

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
        /// Called by the observable to notify this observer that the event has occurred, providing the event payload.
        /// </summary>
        /// <param name="arg">The payload or data associated with the event notification.</param>
        /// <remarks>
        /// This virtual method is invoked when the observed event is triggered, receiving the typed payload.
        /// By default, it does nothing, but derived classes should override it to implement specific reactions
        /// to the event. This method forms the core of the typed observer pattern, allowing objects to respond
        /// to events with additional context data without direct coupling to the event source.
        /// </remarks>
        public virtual void Notified(T arg) { }
    }
}
