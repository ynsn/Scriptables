using System;
using StackMedia.Scriptables.Internal.EasyButtons;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = StackMedia.Scriptables.Editor.Internal.EasyButtons.Button;

namespace StackMedia.Scriptables.Editor
{
    [CustomEditor(typeof(ScriptableEventBase), true, isFallback = false)]
    public class ScriptableEventEditor : UnityEditor.Editor
    {
        private SerializedProperty comment;

        private bool isEditingComment;
        private Button button;

        private void OnEnable()
        {
            comment = serializedObject.FindProperty("comment");

            var invokeMethod = ((ScriptableEventBase)target).GetType().GetMethod("Invoke");
            button = Button.Create(invokeMethod,
                new ButtonAttribute("Invoke"));
        }

        private Vector2 scroll;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorStyles.textField.wordWrap = true;
            if (isEditingComment)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Comment", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                if (GUILayout.Button("Done"))
                {
                    isEditingComment = false;
                }

                EditorGUILayout.EndHorizontal();
                comment.stringValue = EditorGUILayout.TextArea(comment.stringValue, new GUILayoutOption[]
                {
                    GUILayout.ExpandWidth(true),
                    GUILayout.MinHeight(80)
                });
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Comment", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                if (GUILayout.Button("Edit Comment"))
                {
                    isEditingComment = true;
                }

                EditorGUILayout.EndHorizontal();


                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextArea(comment.stringValue, new GUILayoutOption[]
                {
                    GUILayout.ExpandWidth(true),
                    GUILayout.MinHeight(80)
                });
                EditorGUI.EndDisabledGroup();

            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            button.Draw(targets);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
