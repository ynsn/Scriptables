using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    public abstract class ScriptableObserver<T> : ScriptableObserverBehaviour, IObserver<T>

    {
        [SerializeField] private InterfaceReference<IObservable<T>> scriptableEvent;

        [SerializeField] private UnityEvent<T> response = new UnityEvent<T>();

        protected virtual void OnEnable() => scriptableEvent?.Interface?.Subscribe(this);

        protected virtual void OnDisable() => scriptableEvent?.Interface?.Unsubscribe(this);

        public void Notified(T arg)
        {
#if UNITY_EDITOR
            if (DebugEnabled)
            {
                Debug.Log("Invoked", this);
            }
#endif
            response.Invoke(arg);
            if (DisableOnNotifier) enabled = false;
        }
    }
}
