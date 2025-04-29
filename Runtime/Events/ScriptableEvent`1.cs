using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    [GenericOrAbstractType]
    //[CreateAssetMenu]
    public abstract class ScriptableEvent<T> : ScriptableEventBase, IEvent<T>
    {
        protected delegate void EventDelegate(T arg1);

        protected event EventDelegate Event = delegate { };

        public void Invoke(T arg1) => Event.Invoke(arg1);

        public void AddListener(IEventListener<T> listener) => Event += listener.Raise;

        public void RemoveListener(IEventListener<T> listener) => Event -= listener.Raise;
    }
}
