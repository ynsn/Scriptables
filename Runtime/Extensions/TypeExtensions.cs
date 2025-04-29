using System;
using System.Linq;
using UnityEngine;

namespace StackMedia.Scriptables
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the generic type definition of a type if it is a generic type, otherwise returns the type itself.
        /// This is useful for resolving the generic type of a type that may have been constructed with specific type arguments.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The generic type definition if the type is generic, otherwise the type itself.</returns>
        public static Type ResolvedGenericType(this Type type)
        {
            if (!type.IsGenericType) return type;
            Type genericType = type.GetGenericTypeDefinition();
            return genericType != type ? genericType : type;
        }

         public static bool Implements(this Type type, Type interfaceType) => type.GetInterfaces().Any(i => i == interfaceType);
        

        public static bool IsDerivedFrom(this Type type, Type baseType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));

            if (type == baseType) return false;
            if (type.IsSubclassOf(baseType)) return true;

            if (!baseType.IsGenericTypeDefinition) return false;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType) return true;

            Type currentType = type.BaseType;
            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == baseType) return true;
                currentType = currentType.BaseType;
            }

            return false;
        }

        public static string PrettyName(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type.IsGenericType)
            {
                var genericTypeName = type.GetGenericTypeDefinition().Name;
                var components = genericTypeName.Split('`');
                if (components.Length > 1) genericTypeName = components[0];
                var genericArguments = string.Join(", ", type.GetGenericArguments().Select(t => t.PrettyName()));
                return $"{genericTypeName}<{genericArguments}>";
            }

            return type.Name;
        }
    }
}
