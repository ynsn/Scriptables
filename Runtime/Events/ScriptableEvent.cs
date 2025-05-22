using System;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a base class for scriptable events that broadcast notifications without a payload to registered observers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class implements the observer pattern for events that do not require a value or payload.
    /// It allows observers implementing the <see cref="IObserver"/> interface to subscribe and unsubscribe for notifications.
    /// When the <see cref="Notify"/> method is called, all registered observers are notified via their <see cref="IObserver.Notified"/> method.
    /// </para>
    /// <para>
    /// This class is typically used in Unity projects to create decoupled event systems using ScriptableObjects,
    /// enabling communication between components without direct references.
    /// </para>
    /// <para>
    /// For scriptable events that broadcast notifications with a value or payload, see <see cref="ScriptableEvent{T}"/>.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class ScriptableEvent : Scriptable, IObservable, INotifier
    {
        /// <summary>
        /// Represents the delegate signature for event notifications without a payload.
        /// </summary>
        protected delegate void EventDelegate();

        /// <summary>
        /// Occurs when the event is raised, notifying all registered observers.
        /// </summary>
        protected event EventDelegate OnEvent = delegate { };

        /// <summary>
        /// Broadcasts a notification to all registered observers, indicating that the event has occurred.
        /// </summary>
        [ExposeMethod]
        public void Notify() => OnEvent.Invoke();

        /// <summary>
        /// Registers an observer to receive notifications from this event.
        /// </summary>
        /// <param name="listener">
        /// The observer to add to the notification list.
        /// </param>
        public void Subscribe(IObserver listener) => OnEvent += listener.Notified;

        /// <summary>
        /// Unregisters an observer so it no longer receives notifications from this event.
        /// </summary>
        /// <param name="listener">
        /// The observer to remove from the notification list.
        /// </param>
        public void Unsubscribe(IObserver listener) => OnEvent -= listener.Notified;
    }
}
