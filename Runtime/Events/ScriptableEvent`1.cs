using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    public abstract class ScriptableEvent<T> : Scriptable, IObservable<T>, INotifier<T>
    {
        protected delegate void EventDelegate(T arg1);

        protected event EventDelegate Event = delegate { };

        [ExposeMethod]
        public void Notify(T arg1) => Event.Invoke(arg1);

        public void Subscribe(IObserver<T> listener) => Event += listener.Notified;

        public void Unsubscribe(IObserver<T> listener) => Event -= listener.Notified;
    }
}
