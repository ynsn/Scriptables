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
}
