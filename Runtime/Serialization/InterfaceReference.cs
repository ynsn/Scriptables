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
        
        public static implicit operator InterfaceReference<TInterface, TObject>(TObject value) => new InterfaceReference<TInterface, TObject> { value = value };
        
        public static implicit operator TObject(InterfaceReference<TInterface, TObject> reference) => reference.value;
        
        public static implicit operator InterfaceReference<TInterface, TObject>(TInterface value) => new InterfaceReference<TInterface, TObject> { value = value as TObject };
        
        public static implicit operator TInterface(InterfaceReference<TInterface, TObject> reference) => reference.Interface;
        
    }

    [Serializable]
    public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class
    {
    }
}
