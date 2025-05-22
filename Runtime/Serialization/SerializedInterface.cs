using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StackMedia.Scriptables
{
    [Serializable]
    public class SerializedInterface<TInterface, TObject> where TObject : Object where TInterface : class
    {
        [SerializeField, HideInInspector] private TObject value = null as TObject;
        
        public TInterface Value
        {
            get => value switch
            {
                null => null,
                TInterface i => i,
                _ => throw new InvalidOperationException($"{value} is not of type {typeof(TInterface)}")
            };

            set => this.value = (value switch
            {
                null => null,
                TObject o => o,
                    //  _ => throw new InvalidOperationException($"{value} is not of type {typeof(TObject)}")
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            })!;
        }

        public TObject ObjectValue
        {
            get => value;
            set => this.value = value;
        }

        public SerializedInterface()
        {
        }

        public SerializedInterface(TObject value) => this.value = value;

        public SerializedInterface(TInterface value) => this.value = value as TObject;

        public static implicit operator TInterface(SerializedInterface<TInterface, TObject> serializedInterface) => serializedInterface.Value;
        /*public static implicit operator TObject(SerializedInterface<TInterface, TObject> serializedInterface) => serializedInterface.ObjectValue;

        public static implicit operator SerializedInterface<TInterface, TObject>(TInterface type) =>
            new SerializedInterface<TInterface, TObject>(type);
        public static implicit operator SerializedInterface<TInterface, TObject>(TObject type) =>
            new SerializedInterface<TInterface, TObject>(type);*/
    }
    
    [Serializable]
    public class SerializedInterface<TInterface> : SerializedInterface<TInterface, Object> where TInterface : class
    {
    }
}
