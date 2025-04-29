using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    public class ScriptablesSettingsProvider : SettingsProvider
    {

        public ScriptablesSettingsProvider() : base("Project/Scriptables", SettingsScope.Project)
        { }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.LabelField("Scriptables Settings", EditorStyles.boldLabel);

            ScriptablesSettings settings = ScriptablesSettings.instance;
            SerializedObject serializedObject = settings.GetSerializedObject();

            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("typeInstancesFolder"), new GUIContent("Type Instances Folder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("typeInstancesNamespace"), new GUIContent("Type Instances Namespace"));
            if (!EditorGUI.EndChangeCheck())
                return;
            serializedObject.ApplyModifiedProperties();

            
            
            
            settings.Save();
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider() => new ScriptablesSettingsProvider();
    }
}
