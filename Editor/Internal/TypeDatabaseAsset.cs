using System;
using System.Collections.Generic;
using UnityEngine;

namespace StackMedia.Scriptables.Editor.Internal
{
    [SingletonAsset(assetPath: "Settings/Objects", useResources: false)]
    [Serializable]
    internal class TypeDatabaseAsset : SingletonScriptableObject<TypeDatabaseAsset>
    {
        [SerializeField] internal SerializedDictionary<string, List<string>> TypePaths = new SerializedDictionary<string, List<string>>();

        protected override void LoadDefaults()
        {
            base.LoadDefaults();
        }
    }
}
