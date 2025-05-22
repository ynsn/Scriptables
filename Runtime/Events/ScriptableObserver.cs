using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides a concrete implementation of <see cref="ObserverBehaviour"/> that invokes a UnityEvent in response to notifications.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class extends <see cref="ObserverBehaviour"/> to add Unity event system integration, allowing
    /// designers to configure responses to non-typed scriptable events directly in the Unity Inspector. When the observer
    /// receives a notification, it automatically invokes the assigned UnityEvent.
    /// </para>
    /// <para>
    /// ScriptableObserver bridges the gap between the observer pattern and Unity's component-based design by
    /// exposing event responses as inspector-configurable actions. This approach enables non-programmers to create
    /// event-driven behaviors without writing code, while maintaining the architectural benefits of decoupled event systems.
    /// </para>
    /// <para>
    /// While this class is abstract, it provides a complete implementation of the notification handling, so derived classes
    /// typically don't need to override the <see cref="Notified"/> method unless they need to add custom behavior
    /// beyond invoking the UnityEvent.
    /// </para>
    /// <para>
    /// For observers that need to handle events with typed payloads, see <see cref="ScriptableObserver{T}"/>.
    /// </para>
    /// </remarks>
    public abstract class ScriptableObserver : ObserverBehaviour
    {
        /// <summary>
        /// The UnityEvent to invoke when this observer receives a notification.
        /// </summary>
        /// <remarks>
        /// This field is serialized and exposed in the inspector, allowing designers to configure
        /// event responses without coding. The event is invoked whenever the <see cref="Notified"/> method
        /// is called by the observed event. Multiple actions can be assigned to this event in the inspector,
        /// enabling complex reaction chains to a single event notification.
        /// </remarks>
        [SerializeField] private UnityEvent response = new UnityEvent();

        /// <summary>
        /// Called by the observable to notify this observer that the event has occurred.
        /// </summary>
        /// <remarks>
        /// This override implements the notification handling by invoking the <see cref="response"/> UnityEvent.
        /// When the observed event is triggered, this method is called, which in turn triggers all the actions
        /// that have been assigned to the response event in the inspector. This provides a visual, designer-friendly
        /// way to configure reactions to events.
        /// </remarks>
        public override void Notified()
        {
            response.Invoke();
        }
    }
}
