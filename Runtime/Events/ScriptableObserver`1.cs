using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a concrete implementation of <see cref="ObserverBehaviour{T}"/> that invokes a typed UnityEvent in response to notifications.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class extends <see cref="ObserverBehaviour{T}"/> to add Unity event system integration, allowing
    /// designers to configure responses to typed scriptable events directly in the Unity Inspector. When the observer
    /// receives a notification with a payload of type <typeparamref name="T"/>, it automatically invokes the assigned
    /// UnityEvent, passing along the payload data.
    /// </para>
    /// <para>
    /// ScriptableObserver&lt;T&gt; bridges the gap between the observer pattern and Unity's component-based design by
    /// exposing event responses as inspector-configurable actions. This approach enables non-programmers to create
    /// event-driven behaviors without writing code, while maintaining the architectural benefits of decoupled event systems
    /// and providing type safety for event data.
    /// </para>
    /// <para>
    /// While this class is abstract, it provides a complete implementation of the notification handling, so derived classes
    /// typically don't need to override the <see cref="Notified(T)"/> method unless they need to add custom behavior
    /// beyond invoking the UnityEvent.
    /// </para>
    /// <para>
    /// For observers that need to handle events without typed payloads, see <see cref="ScriptableObserver"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the payload that this observer can receive and process.</typeparam>
    public abstract class ScriptableObserver<T> : ObserverBehaviour<T>
    {
        /// <summary>
        /// The typed UnityEvent to invoke when this observer receives a notification.
        /// </summary>
        /// <remarks>
        /// This field is serialized and exposed in the inspector, allowing designers to configure
        /// event responses without coding. The event is invoked whenever the <see cref="Notified(T)"/> method
        /// is called by the observed event, with the payload passed as an argument to all event handlers.
        /// Multiple actions can be assigned to this event in the inspector, enabling complex reaction chains
        /// to a single event notification with access to the event data.
        /// </remarks>
        [SerializeField] private UnityEvent<T> response = new UnityEvent<T>();

        /// <summary>
        /// Called by the observable to notify this observer that the event has occurred, providing the event payload.
        /// </summary>
        /// <param name="arg">The payload or data associated with the event notification.</param>
        /// <remarks>
        /// This override implements the notification handling by invoking the <see cref="response"/> UnityEvent
        /// with the provided argument. When the observed event is triggered, this method is called with the event data,
        /// which in turn is passed to all the actions that have been assigned to the response event in the inspector.
        /// This provides a visual, designer-friendly way to configure reactions to events while maintaining
        /// type safety for the event data.
        /// </remarks>
        public override void Notified(T arg)
        {
            response.Invoke(arg);
        }
    }
}
