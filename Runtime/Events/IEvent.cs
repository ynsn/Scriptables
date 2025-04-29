using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    public interface IEventBase
    {
        Type[] Types { get; }
    }

    public interface IEvent : IEventBase
    {
        Type[] IEventBase.Types => Type.EmptyTypes;

        void Invoke();
        void AddListener(IEventListener listener);
        void RemoveListener(IEventListener listener);

        IDisposable Subscribe(Action action);

        /*IEvent Forward();*/
    }

    public interface IEvent<T> : IEventBase
    {
        Type[] IEventBase.Types => new[] { typeof(T) };

        void Invoke(T value);
        void AddListener(IEventListener<T> listener);
        void RemoveListener(IEventListener<T> listener);
    }

    public interface IEvent<T1, T2> : IEventBase
    {
        Type[] IEventBase.Types => new[] { typeof(T1), typeof(T2) };

        void Invoke(T1 value1, T2 value2);
        void AddListener(IEventListener<T1, T2> listener);
        void RemoveListener(IEventListener<T1, T2> listener);
    }
}
