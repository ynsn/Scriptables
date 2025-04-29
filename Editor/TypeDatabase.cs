using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    public sealed class TypeDatabase
    {
        [InitializeOnLoadMethod]
        private static void WOw()
        {
            foreach (Type genericAssetType in GetGenericAssetTypes())
            {
                Debug.Log(genericAssetType.Name);
            }
        }

        public static Type[] GetGenericAssetTypes()
        {
            TypeCache.TypeCollection types = TypeCache.GetTypesWithAttribute<GenericOrAbstractTypeAttribute>();
            var filteredTypes = types.Select(x => x).Where(x => x.GetCustomAttribute<CreateAssetMenuAttribute>() != null)
                .Where(x => x.IsSubclassOf(typeof(ScriptableObject))).ToArray();
            return filteredTypes;
        }
    }
}
