namespace StackMedia.Scriptables
{
    /// <summary>
    /// Defines a contract for observable objects that allow observers to subscribe and unsubscribe for notifications with a payload of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value or payload that will be sent to observers upon notification.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// Implement this interface to enable an object to manage a list of observers interested in receiving notifications
    /// about changes, events, or state updates, along with relevant data of type <typeparamref name="T"/>.
    /// The <see cref="Subscribe"/> method registers an observer, while <see cref="Unsubscribe"/> removes it from the notification list.
    /// </para>
    /// <para>
    /// This interface is commonly used in the generic observer pattern, facilitating decoupled communication between
    /// the observable (subject) and its observers (listeners). Observers typically implement the <see cref="IObserver{T}"/>
    /// interface to receive notifications with a value or payload.
    /// </para>
    /// <para>
    /// For observable objects that notify observers without a value or payload, see <see cref="IObservable"/>.
    /// </para>
    /// </remarks>
    public interface IObservable<out T>
    {
        /// <summary>
        /// Registers an observer to receive notifications with a payload of type <typeparamref name="T"/> from this observable.
        /// </summary>
        /// <param name="observer">
        /// The observer to add to the notification list.
        /// </param>
        void Subscribe(IObserver<T> observer);

        /// <summary>
        /// Unregisters an observer so it no longer receives notifications with a payload of type <typeparamref name="T"/> from this observable.
        /// </summary>
        /// <param name="observer">
        /// The observer to remove from the notification list.
        /// </param>
        void Unsubscribe(IObserver<T> observer);
    }
}
