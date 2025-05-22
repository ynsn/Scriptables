using UnityEngine;

namespace StackMedia.Scriptables
{
    public interface IObservable
    {
        void Subscribe(IObserver observer);
        void Unsubscribe(IObserver observer);
    }
    
    public interface IObservable<out T>
    {
        void Subscribe(IObserver<T> observer);
        void Unsubscribe(IObserver<T> observer);
    }
}
