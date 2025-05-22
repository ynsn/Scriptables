using UnityEngine;

namespace StackMedia.Scriptables
{
    public interface IObserver
    {
        void Notified();
    }

    public interface IObserver<in T>
    {
        void Notified(T value);
    }
}
