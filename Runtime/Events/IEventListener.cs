using UnityEngine;

namespace StackMedia.Scriptables
{
    public interface IEventListener
    {
        IEvent Event { get; }
        
        void Raise();
    }

    public interface IEventListener<T>
    {
        IEvent<T> Event { get; }
        
        void Raise(T value);
    }

    public interface IEventListener<T1, T2>
    {
        IEvent<T1, T2> Event { get; }
        
        void Raise(T1 value1, T2 value2);
    }
}
