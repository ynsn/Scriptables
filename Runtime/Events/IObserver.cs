namespace StackMedia.Scriptables
{
    /// <summary>
    /// Defines a contract for observers that receive notifications without a payload from an observable object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implement this interface to allow an object to react to notifications or events signaled by an observable,
    /// without requiring any additional data. The <see cref="Notified"/> method is called by the observable
    /// to inform the observer that a relevant event or state change has occurred.
    /// </para>
    /// <para>
    /// This interface is commonly used in the observer pattern, enabling decoupled communication between
    /// the observable (subject) and its observers (listeners). For observers that require a value or payload
    /// with the notification, see <see cref="IObserver{T}"/>.
    /// </para>
    /// </remarks>
    public interface IObserver
    {
        /// <summary>
        /// Called by the observable to notify this observer of an event or state change, without any payload.
        /// </summary>
        void Notified();
    }

    
}
