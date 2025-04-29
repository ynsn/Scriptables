using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    public abstract class ScriptableEventListener : ScriptableEventListenerBase, IEventListener
    {
        [SerializeField] private SerializedInterface<IEvent> scriptableEvent;

        [SerializeField] private UnityEvent response = new UnityEvent();

        public IEvent Event => scriptableEvent.Value;

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnEnable() => scriptableEvent.Value.AddListener(this);

        protected virtual void OnDisable() => scriptableEvent.Value.RemoveListener(this);

        public void Raise()
        {
#if UNITY_EDITOR
            if (enableDebug)
            {
                Debug.Log("Invoked", this);
            }
#endif

            SendMessage("OnRaised",  SendMessageOptions.DontRequireReceiver);
            if (responseDelay > 0.01)
            {
                Invoke(nameof(response.Invoke), responseDelay);
            }
            else
            {
                response.Invoke();
            }
        }
    }
}
