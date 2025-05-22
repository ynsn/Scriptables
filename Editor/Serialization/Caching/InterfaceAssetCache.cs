using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    [FilePath("Caches/InterfaceAssetCache.asset", FilePathAttribute.Location.ProjectFolder)]
    public class InterfaceAssetCache : ScriptableSingleton<InterfaceAssetCache>
    {

        [SerializeField] private SerializedDictionary<string, List<string>> typePaths = new SerializedDictionary<string, List<string>>();

        public void Invalidate()
        {
            typePaths.Clear();
        }

        public void Invalidate(string path)
        {
            if (typePaths.ContainsKey(path))
            {
                typePaths.Remove(path);
                Debug.Log("Invalidated path: " + path);
            }
        }

        public List<string> GetPaths(Type type)
        {
            typePaths.Clear();
            /*if (typePaths.ContainsKey(type.AssemblyQualifiedName))
            {
                return typePaths[type.AssemblyQualifiedName];
            }
            else
            {*/
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
           //}
        }
    }
}
