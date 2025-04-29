using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    [GenericOrAbstractType]
    //[CreateAssetMenu]
    public abstract class ScriptableEvent : ScriptableEventBase, IEvent
    {
        protected delegate void EventDelegate();

        protected event EventDelegate Event = delegate { };

        public void Invoke() => Event.Invoke();

        public void AddListener(IEventListener listener) => Event += listener.Raise;

        public void RemoveListener(IEventListener listener) => Event -= listener.Raise;
        
        public IDisposable Subscribe(Action action)
        {
            EventDelegate eventDelegate = () => action?.Invoke();
            Event += eventDelegate;
            return new Subscription(() => Event -= eventDelegate);
        }
    }
}
