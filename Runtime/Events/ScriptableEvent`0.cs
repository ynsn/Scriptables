using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    public abstract class ScriptableEvent : Scriptable, IObservable, INotifier
    {
        protected delegate void EventDelegate();

        protected event EventDelegate Event = delegate { };

        [ExposeMethod]
        public void Notify() => Event.Invoke();

        public void Subscribe(IObserver listener) => Event += listener.Notified;

        public void Unsubscribe(IObserver listener) => Event -= listener.Notified;
    }
}
