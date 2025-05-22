using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    public abstract class ScriptableObserver : ScriptableObserverBehaviour, IObserver
    {
        [SerializeField] private InterfaceReference<IObservable> scriptableEvent;

        [SerializeField] private UnityEvent response = new UnityEvent();
        
        protected virtual void OnEnable() => scriptableEvent?.Interface?.Subscribe(this);

        protected virtual void OnDisable() => scriptableEvent?.Interface?.Unsubscribe(this);

        public void Notified()
        {
#if UNITY_EDITOR
            if (DebugEnabled) Debug.Log($"{gameObject.name} notified by {scriptableEvent.Object.name}", this);
#endif
            response.Invoke();
            if (DisableOnNotifier) enabled = false;
        }
    }
}
