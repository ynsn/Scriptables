using System;
using UnityEngine;

namespace StackMedia.Scriptables
{
    public interface INotifier
    {
        void Notify();
    }

    public interface IAsyncNotifier : IAsyncDisposable
    {
        Awaitable NotifyAsync();
    }

    public interface INotifier<in T>
    {
        void Notify(T value);
    }

    public interface IAsyncNotifier<in T> : IAsyncDisposable
    {
        Awaitable NotifyAsync(T value);
    }
}
