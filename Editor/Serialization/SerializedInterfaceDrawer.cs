using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using StackMedia.Scriptables.Editor.UI;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace StackMedia.Scriptables.Editor
{
    internal static class PropertyEditorUtility
    {
        internal static void Show(Object obj)
        {
            Type propertyEditor = typeof(EditorWindow).Assembly.GetTypes()
                .FirstOrDefault(x => x.Name == "PropertyEditor");

            if (propertyEditor == null)
                return;

            MethodInfo openPropertyEditorMethod = propertyEditor.GetMethod("OpenPropertyEditor",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[]
                {
                    typeof(UnityEngine.Object),
                    typeof(bool)
                },
                null);

            openPropertyEditorMethod.Invoke(null,
                new object[]
                {
                    obj, true
                });
        }
    }

    [CustomPropertyDrawer(typeof(SerializedInterface<>))]
    [CustomPropertyDrawer(typeof(SerializedInterface<,>))]
    public class SerializedInterfaceDrawer : PropertyDrawer
    {
        private SerializedProperty valueProperty;
        private FuzzyMenu<string> fuzzyMenu;
        private Type objectType;
        private Type interfaceType;
        private Rect objectFieldRect;
        private int controlId;

        private bool HasKeyboardFocus => GUIUtility.keyboardControl == controlId;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);

            EditorGUI.BeginProperty(position, label, property);
            objectFieldRect = DrawLabel(position, label.text);
            DrawObjectField(objectFieldRect);
            EditorGUI.EndProperty();

            ProcessEvents(position, objectFieldRect, property);
        }

        private void Initialize(SerializedProperty property)
        {
            ResolveInterfaceArgs();
            valueProperty ??= property.FindPropertyRelative("value");
        }

        private FuzzyMenuItem<string> BuildRoot()
        {
            var root = new FuzzyMenuItem<string>(interfaceType.Name);
            /*foreach (var assetPath in InterfaceAssetCache.Instance.GetPaths(interfaceType))
            {
                var item = new FuzzyMenuItem<Object>(assetPath)
                {
                    Icon = AssetDatabase.GetCachedIcon(assetPath) as Texture2D
                };
                root.AddChild(item);
            }*/
            
           

            var noneItem = root.AddChild("None");
            noneItem.UserData = null;

            var assetsGroupItem = root.AddChild("Assets");
            assetsGroupItem.AddChild("WELP");

            foreach (var path in InterfaceAssetCache.instance.GetPaths(interfaceType))
            {
                var assetName = Path.GetFileNameWithoutExtension(path);

                var item = new FuzzyMenuItem<string>(assetName)
                {
                    Icon = AssetDatabase.GetCachedIcon(path) as Texture2D,
                    UserData = path
                };
                assetsGroupItem.AddChild(item);
            }

            return root;
        }

        private Rect DrawLabel(Rect position, string label)
        {
            controlId = GUIUtility.GetControlID(FocusType.Keyboard);
            Rect rect = EditorGUI.PrefixLabel(position, controlId, new GUIContent(label));

            return new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        }

        private void DrawObjectField(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var fieldText = valueProperty.objectReferenceValue?.ToString() ?? $"None ({interfaceType.PrettyName()})";
                Texture icon = valueProperty.objectReferenceValue != null
                    ? EditorGUIUtility.ObjectContent(valueProperty.objectReferenceValue, valueProperty.objectReferenceValue.GetType()).image
                    : null;

                EditorStyles.objectField.Draw(position, new GUIContent(fieldText, icon), position.Contains(Event.current.mousePosition), false,
                    false,
                    HasKeyboardFocus);

            }

            var buttonRect = new Rect(position);
            buttonRect.xMin = buttonRect.xMax - 20;
            buttonRect.xMax -= 1;
            buttonRect.yMin += 1;
            buttonRect.yMax -= 1;
            if (GUI.Button(buttonRect, string.Empty, "objectFieldButton"))
            {
                fuzzyMenu = new FuzzyMenu<string>(BuildRoot());
                fuzzyMenu.OnItemSelected = (item) =>
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        valueProperty.objectReferenceValue = null;
                        valueProperty.serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    valueProperty.objectReferenceValue = AssetDatabase.LoadMainAssetAtPath(item);
                    valueProperty.serializedObject.ApplyModifiedProperties();
                };
                fuzzyMenu.Show(GUIUtility.GUIToScreenRect(position));

                //dropdown = new InterfaceDropdown(new AdvancedDropdownState(), interfaceType, valueProperty);
                //dropdown.Show(position);
            }
        }


        private void ProcessEvents(Rect position, Rect objectFieldPosition, SerializedProperty property)
        {
            if (objectFieldPosition.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        var isValid =
                            (from draggedObject in DragAndDrop.objectReferences where draggedObject != null select draggedObject.GetType()).Any(draggedType =>
                                draggedType.Implements(interfaceType));

                        if (isValid)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            Event.current.Use();
                        }

                        break;
                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();
                        if (DragAndDrop.objectReferences.Length == 1)
                        {
                            Object draggedObject = DragAndDrop.objectReferences[0];
                            if (draggedObject != null)
                            {
                                if (draggedObject.GetType().Implements(interfaceType))
                                {
                                    valueProperty.objectReferenceValue = draggedObject;
                                    property.serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }

                        break;
                    case EventType.MouseDown:

                        if (Event.current.button == 0)
                        {
                            GUIUtility.keyboardControl = controlId;

                            // Ping the object in the inspector
                            if (valueProperty.objectReferenceValue != null)
                            {
                                if (Event.current.clickCount == 2)
                                {
                                    if (GUIUtility.keyboardControl == controlId && valueProperty.objectReferenceValue != null)
                                    {
                                        // Open the object in the inspector
                                        Selection.activeObject = valueProperty.objectReferenceValue;
                                        return;
                                    }
                                }

                                EditorGUIUtility.PingObject(valueProperty.objectReferenceValue);
                                Helpers.RepaintActiveEditors();
                            }
                        }
                        else if (Event.current.button == 1)
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Clear"), false, () =>
                            {
                                valueProperty.objectReferenceValue = null;
                                property.serializedObject.ApplyModifiedProperties();
                            });
                            if (valueProperty.objectReferenceValue != null)
                            {
                                menu.AddItem(new GUIContent("Properties"), false, () => { PropertyEditorUtility.Show(valueProperty.objectReferenceValue); });
                            }

                            menu.ShowAsContext();
                            Event.current.Use();
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private void ResolveInterfaceArgs()
        {
            if (objectType != null && interfaceType != null) return;
            Type field = fieldInfo.FieldType;
            if (field.IsGenericType)
            {
                var genericArguments = field.GetGenericArguments();
                if (genericArguments.Length == 2)
                {
                    objectType = genericArguments[1];
                    interfaceType = genericArguments[0];
                }
                else if (genericArguments.Length == 1)
                {
                    objectType = typeof(Object);
                    interfaceType = genericArguments[0];
                }
            }
            else
            {
                objectType = typeof(Object);
                interfaceType = field;
            }
        }
    }
}
