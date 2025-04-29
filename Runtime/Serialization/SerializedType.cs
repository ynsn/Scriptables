using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace StackMedia.Scriptables
{
    [Flags]
    public enum TypeFlags
    {
        None = 0,
        Classes = 1 << 0,
        Interfaces = 1 << 1,
        Structs = 1 << 2,
        Enums = 1 << 3,
        All = Classes | Interfaces | Structs
    }

    /// <summary>
    /// Attribute to specify that a SerializedType must implement a specific interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DerivedFromAttribute : PropertyAttribute
    {
        public Func<Type, bool> Predicate { get; }

        /// <summary>
        /// The type that the serialized type must implement.
        /// </summary>
        public Type ImplementedType { get; }

        /// <summary>
        /// The interface type that the serialized type must implement.
        /// This constructor mandates that the type is an interface.
        /// </summary>
        /// <param name="type">The interface type that the serialized type must implement.</param>
        public DerivedFromAttribute(Type type)
        {
            ImplementedType = type;
            Predicate = p => p.IsDerivedFrom(type);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TypePropertiesAttribute : PropertyAttribute
    {
        public Func<Type, bool> Predicate { get; }

        public TypeFlags TypeFlags { get; }

        public TypePropertiesAttribute(TypeFlags typeFlags)
        {
            TypeFlags = typeFlags;
            Predicate = type =>
            {
                if (type.IsClass && typeFlags.HasFlag(TypeFlags.Classes)) return true;
                if (type.IsInterface && typeFlags.HasFlag(TypeFlags.Interfaces)) return true;
                if (type.IsValueType && !type.IsEnum && typeFlags.HasFlag(TypeFlags.Structs)) return true;
                if (type.IsEnum && type.IsValueType && typeFlags.HasFlag(TypeFlags.Enums)) return true;
                return false;
            };
        }
    }

    [Serializable]
    public class SerializedType : ISerializationCallbackReceiver
    {
        [SerializeField] internal string assemblyQualifiedTypeName;

        [NonSerialized] private Type type;

        public Type Type
        {
            get
            {
                if (type != null) return type;
                type = Type.GetType(assemblyQualifiedTypeName, false);
                return type;
            }

            set
            {
                type = value;
                assemblyQualifiedTypeName = value?.AssemblyQualifiedName;
            }
        }

        public string FullName => Type?.FullName;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            type = null;
        }

        public static implicit operator Type(SerializedType serializedType) => serializedType.Type;
        public static implicit operator SerializedType(Type type) => new SerializedType { Type = type };

    }
}
