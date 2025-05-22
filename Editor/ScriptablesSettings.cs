using System.IO;
using Mono.Cecil;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    [FilePath("ProjectSettings/ScriptablesSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class ScriptablesSettings : UnityEditor.ScriptableSingleton<ScriptablesSettings>
    {
        [SerializeField] private string artifactsAssetFolder = "Scriptables.Generated~/";
        
        //[SerializeField] private bool generateAssemblyDefinition = true;

       // [SerializeField] private string assemblyDefinitionName = "StackMedia.Scriptables.Generated";
        
        [SerializeField] private string typeInstancesNamespace = "StackMedia.Scriptables.Generated";

        public string ArtifactsAssetFolder
        {
            get => artifactsAssetFolder;

            set
            {
                artifactsAssetFolder = value;
                Save();
            }
        }

        public string TypeInstancesNamespace
        {
            get => typeInstancesNamespace;

            set
            {
                typeInstancesNamespace = value;
                Save();
            }
        }

        private void OnEnable()
        {
           
        }

        private void OnDisable()
        {
            Save();
            
        }
        

        public void Save()
        {
            Save(true);
        }

        internal SerializedObject GetSerializedObject()
        {
            return new SerializedObject(this);
        }
    }
}
