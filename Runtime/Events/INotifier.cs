namespace StackMedia.Scriptables
{
    /// <summary>
    /// Defines a contract for objects that can broadcast notifications without payloads.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implement this interface to allow an object to notify subscribers or observers of an event or state change.
    /// The <see cref="Notify"/> method is typically called to signal that something of interest has occurred,
    /// without providing any additional data.
    /// </para>
    /// <para>
    /// This interface is commonly used in event-driven or observer patterns, where decoupled components
    /// need to react to changes or actions in other parts of the system.
    /// </para>
    /// <para>
    /// For notifications that include a value or payload, see <see cref="INotifier{T}"/>.
    /// </para>
    /// </remarks>
    public interface INotifier
    {
        /// <summary>
        /// Broadcasts a notification to all registered observers or listeners.
        /// </summary>
        void Notify();
    }

    /// <summary>
    /// Defines a contract for objects that can broadcast notifications with a payload of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value or payload to be sent with the notification.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// Implement this interface to allow an object to notify subscribers or observers of an event or state change,
    /// providing additional data relevant to the notification.
    /// The <see cref="Notify(T)"/> method is typically called to signal that something of interest has occurred,
    /// along with a value describing the event.
    /// </para>
    /// <para>
    /// This interface is commonly used in event-driven or observer patterns, where decoupled components
    /// need to react to changes or actions in other parts of the system and require context or data about the event.
    /// </para>
    /// <para>
    /// For notifications that do not include a value or payload, see <see cref="INotifier"/>.
    /// </para>
    /// </remarks>
    public interface INotifier<in T>
    {
        /// <summary>
        /// Broadcasts a notification with the specified value to all registered observers or listeners.
        /// </summary>
        /// <param name="value">
        /// The value or payload to send with the notification.
        /// </param>
        void Notify(T value);
    }
}
