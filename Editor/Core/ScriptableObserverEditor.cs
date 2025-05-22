using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    [CustomEditor(typeof(ScriptableObserver<>), true)]
    public class ScriptableObserverEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
