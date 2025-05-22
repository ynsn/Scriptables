using System;
using System.Collections.Generic;
using StackMedia.Scriptables.Editor.Internal;

namespace StackMedia.Scriptables.Editor
{
    

    public static class TypeDatabase
    {
        public static void AddInterfaceCache(Type interfaceType, Type concreteType)
        {
            if (TypeDatabaseAsset.Instance.TypePaths.ContainsKey(interfaceType.AssemblyQualifiedName))
            {
                if (TypeDatabaseAsset.Instance.TypePaths[interfaceType.AssemblyQualifiedName] == null)
                {
                    TypeDatabaseAsset.Instance.TypePaths[interfaceType.AssemblyQualifiedName] = new List<string>();
                }
            }
        }

        public static void Save()
        {
            
        }
    }
}
