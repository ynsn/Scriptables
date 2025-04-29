using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    public class InterfaceAssetCache : ScriptableObject
    {
        private static InterfaceAssetCache instance;

        public static InterfaceAssetCache Instance
        {
            get
            {
                if (instance == null)
                    instance = LoadOrCreateInstance();
                return instance;
            }
        }

        private static InterfaceAssetCache LoadOrCreateInstance()
        {
            var inst = Resources.FindObjectsOfTypeAll<InterfaceAssetCache>();
            if (inst != null && inst.Length > 0)
            {
                if (inst.Length > 1)
                {
                    Debug.LogWarning($"Multiple InterfaceAssetCache instances of type {inst[0].GetType().Name}");
                    
                    // Destroy all but the first instance
                    for (int i = 1; i < inst.Length; i++)
                    {
                        DestroyImmediate(inst[i]);
                    }
                    
                    Debug.LogWarning($"Destroyed {inst.Length - 1} duplicate instances of {inst[0].GetType().Name}");
                }
                
                Debug.Log("Found existing InterfaceAssetCache instance");
                return inst[0];
            }

            instance = CreateInstance<InterfaceAssetCache>();
            instance.name = "InterfaceAssetCache";
            if (!Directory.Exists("Assets/Scriptables.Generated"))
            {
                Directory.CreateDirectory("Assets/Scriptables.Generated");
            }
            AssetDatabase.CreateAsset(instance, "Assets/Scriptables.Generated/InterfaceAssetCache.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return instance;
        }
        
        [SerializeField] private SerializedDictionary<string, List<string>> typePaths = new SerializedDictionary<string, List<string>>();

        public List<string> GetPaths(Type type)
        {
            if (typePaths.ContainsKey(type.AssemblyQualifiedName)) 
            {
                return typePaths[type.AssemblyQualifiedName];
            }
            else
            {
                var paths = new List<string>();
                var allAssetPaths = AssetDatabase.GetAllAssetPaths();
                foreach (var assetPath in allAssetPaths)
                {
                    Type asset = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                    if (type.IsAssignableFrom(asset))
                    {
                        paths.Add(assetPath);
                    }
                }
                typePaths.Add(type.AssemblyQualifiedName, paths);
                return paths;
            }
        }
    }
}
