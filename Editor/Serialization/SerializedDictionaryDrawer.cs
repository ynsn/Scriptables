using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace StackMedia.Scriptables.Editor
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
    public class SerializedDictionaryDrawer : PropertyDrawer
    {

        private ReorderableList reorderableList;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            reorderableList ??= new ReorderableList(property.serializedObject, property.FindPropertyRelative("items"), true, true, true, true);

            reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, label);

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                var keyProperty = element.FindPropertyRelative("key");
                var valueProperty = element.FindPropertyRelative("value");
                var duplicateKeyProperty = element.FindPropertyRelative("duplicateKey");

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                if (!duplicateKeyProperty.boolValue)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 2 - 5, rect.height), keyProperty, GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, rect.height), valueProperty, GUIContent.none);
                }
                else
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 2 - 5, rect.height), keyProperty, GUIContent.none);
                    EditorGUI.HelpBox(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 25, rect.height), "Duplicate Key", MessageType.Error);
                }
            };

            reorderableList.onAddCallback = (list) =>
            {
                var index = list.serializedProperty.arraySize;
                list.serializedProperty.InsertArrayElementAtIndex(index);
            };


            // EditorGUI.BeginProperty(position, label, property);
            var height = EditorGUI.GetPropertyHeight(reorderableList.serializedProperty);


            reorderableList.DoList(new Rect(position.x, position.y, position.width, height));

            //EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            reorderableList ??= new ReorderableList(property.serializedObject, property.FindPropertyRelative("items"), true, true, true, true);
            return reorderableList.GetHeight();
        }
    }
}
