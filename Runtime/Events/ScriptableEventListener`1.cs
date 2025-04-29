using System;
using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    public abstract class ScriptableEventListener<T> : ScriptableEventListenerBase, IEventListener<T>
    {
        [SerializeField] private SerializedInterface<IEvent<T>> scriptableEvent;

        [SerializeField] private UnityEvent<T> response = new UnityEvent<T>();

        public IEvent<T> Event => scriptableEvent.Value;

        protected virtual void OnEnable() => scriptableEvent?.Value.AddListener(this);

        protected virtual void OnDisable() => scriptableEvent?.Value.RemoveListener(this);

        public void Raise(T arg)
        {
#if UNITY_EDITOR
            if (enableDebug)
            {
                Debug.Log("Invoked", this);
            }
#endif
            if (responseDelay > 0.01)
            {
                Invoke(nameof(DelayInvoke), responseDelay);
            }
            else
            {
                response.Invoke(arg);
            }
        }
        
        private void DelayInvoke(T arg)
        {
            response.Invoke(arg);
        }
    }
}
