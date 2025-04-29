using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StackMedia.Scriptables.Editor
{
    [CustomPropertyDrawer(typeof(SerializedKeyValuePair<,>))]
    public class SerializedKeyValuePairDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty keyProperty = property.FindPropertyRelative("Key");
            SerializedProperty valueProperty = property.FindPropertyRelative("Value");

            var keyLabel = new GUIContent(keyProperty.displayName);
            var valueLabel = new GUIContent(valueProperty.displayName);

            var keyRect = new Rect(position.x, position.y, position.width / 2, position.height);
            var valueRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);
            
            EditorGUI.PropertyField(keyRect, keyProperty);
            EditorGUI.PropertyField(valueRect, valueProperty);
        }
    }
}
