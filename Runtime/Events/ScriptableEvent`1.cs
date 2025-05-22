using System;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a base class for scriptable events that broadcast notifications with a payload of type <typeparamref name="T"/> to registered observers.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value or payload sent to observers upon notification.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// This abstract class implements the generic observer pattern for events that require a value or payload.
    /// It allows observers implementing the <see cref="IObserver{T}"/> interface to subscribe and unsubscribe for notifications.
    /// When the <see cref="Notify(T)"/> method is called, all registered observers are notified via their <see cref="IObserver{T}.Notified(T)"/> method, receiving the provided value.
    /// </para>
    /// <para>
    /// This class is typically used in Unity projects to create decoupled event systems using ScriptableObjects,
    /// enabling communication between components without direct references, and allowing events to carry contextual data.
    /// </para>
    /// <para>
    /// For scriptable events that broadcast notifications without a value or payload, see <see cref="ScriptableEvent"/>.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class ScriptableEvent<T> : Scriptable, IObservable<T>, INotifier<T>
    {
        /// <summary>
        /// Represents the delegate signature for event notifications with a payload of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="arg1">
        /// The value or payload associated with the notification.
        /// </param>
        protected delegate void EventDelegate(T arg1);

        /// <summary>
        /// Occurs when the event is raised, notifying all registered observers with a payload of type <typeparamref name="T"/>.
        /// </summary>
        protected event EventDelegate OnEvent = delegate { };

        /// <summary>
        /// Broadcasts a notification to all registered observers, providing a payload of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="arg1">
        /// The value or payload to send to observers.
        /// </param>
        [ExposeMethod]
        public void Notify(T arg1) => OnEvent.Invoke(arg1);

        /// <summary>
        /// Registers an observer to receive notifications with a payload of type <typeparamref name="T"/> from this event.
        /// </summary>
        /// <param name="listener">
        /// The observer to add to the notification list.
        /// </param>
        public void Subscribe(IObserver<T> listener) => OnEvent += listener.Notified;

        /// <summary>
        /// Unregisters an observer so it no longer receives notifications with a payload of type <typeparamref name="T"/> from this event.
        /// </summary>
        /// <param name="listener">
        /// The observer to remove from the notification list.
        /// </param>
        public void Unsubscribe(IObserver<T> listener) => OnEvent -= listener.Notified;
    }
}
