using System;
using UnityEngine;
using UnityEngine.Events;

namespace StackMedia.Scriptables
{
    public class Subscription : IDisposable
    {
        private readonly UnityAction onDispose;

        public Subscription(UnityAction onDispose) => this.onDispose = onDispose;
        
        public void Dispose() => onDispose?.Invoke();
    }
}
