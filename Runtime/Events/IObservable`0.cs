namespace StackMedia.Scriptables
{
    /// <summary>
    /// Defines a contract for observable objects that allow observers to subscribe and unsubscribe for notifications.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implement this interface to enable an object to manage a list of observers interested in receiving notifications
    /// about changes, events, or state updates. The <see cref="Subscribe"/> method registers an observer, while
    /// <see cref="Unsubscribe"/> removes it from the notification list.
    /// </para>
    /// <para>
    /// This interface is commonly used in the observer pattern, facilitating decoupled communication between
    /// the observable (subject) and its observers (listeners). Observers typically implement the <see cref="IObserver"/>
    /// interface to receive notifications.
    /// </para>
    /// <para>
    /// For observable objects that notify observers with a value or payload, see <see cref="IObservable{T}"/>.
    /// </para>
    /// </remarks>
    public interface IObservable
    {
        /// <summary>
        /// Registers an observer to receive notifications from this observable.
        /// </summary>
        /// <param name="observer">
        /// The observer to add to the notification list.
        /// </param>
        void Subscribe(IObserver observer);

        /// <summary>
        /// Unregisters an observer so it no longer receives notifications from this observable.
        /// </summary>
        /// <param name="observer">
        /// The observer to remove from the notification list.
        /// </param>
        void Unsubscribe(IObserver observer);
    }
}
