using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StackMedia.Scriptables
{
    [Serializable]
    public class InterfaceReference<TInterface, TObject> : ISerializationCallbackReceiver where TInterface : class where TObject : class
    {
        [SerializeField] private TObject value;

        [NonSerialized] private TInterface cachedValue;

        public TObject Object => value;

        public TInterface Interface => cachedValue ??= value as TInterface;

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() => cachedValue = value as TInterface;
    }

    [Serializable]
    public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class
    {
    }
}
