using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    [CustomEditor(typeof(ScriptableComponent), true)]
    public class ScriptableComponentDefaultEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
