namespace StackMedia.Scriptables
{
    /// <summary>
    /// Defines a contract for observers that receive notifications with a payload of type <typeparamref name="T"/> from an observable object.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value or payload received with the notification.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// Implement this interface to allow an object to react to notifications or events signaled by an observable,
    /// receiving additional data relevant to the event. The <see cref="Notified(T)"/> method is called by the observable
    /// to inform the observer that a relevant event or state change has occurred, along with a value describing the event.
    /// </para>
    /// <para>
    /// This interface is commonly used in the generic observer pattern, enabling decoupled communication between
    /// the observable (subject) and its observers (listeners). For observers that do not require a value or payload
    /// with the notification, see <see cref="IObserver"/>.
    /// </para>
    /// </remarks>
    public interface IObserver<in T>
    {
        /// <summary>
        /// Called by the observable to notify this observer of an event or state change, providing a payload of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">
        /// The value or payload associated with the notification.
        /// </param>
        void Notified(T value);
    }
}
